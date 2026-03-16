using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace ConsistentDoubleJump;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("JumpCount")]
    public int JumpCount { get; set; } = 2;

    [JsonPropertyName("JumpVelocity")]
    public float JumpVelocity { get; set; } = 280.0f;

    [JsonPropertyName("HorizontalBoost")]
    public float HorizontalBoost { get; set; } = 1.2f;

    [JsonPropertyName("BhopWindowTicks")]
    public int BhopWindowTicks { get; set; } = 8;
}

public class ConsistentDoubleJump : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "ConsistentDoubleJump";
    public override string ModuleVersion => "0.5.0";
    public override string ModuleAuthor => "Vqesh";
    public override string ModuleDescription => "Double jump + bhop assist for CS2";

    public PluginConfig Config { get; set; } = new();

    private readonly Dictionary<int, PlayerJumpState> _players = new();

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
        Logger.LogInformation("[DJ] Config: JumpCount={Count} Velocity={Vel} HBoost={Boost} BhopWindow={Window}",
            config.JumpCount, config.JumpVelocity, config.HorizontalBoost, config.BhopWindowTicks);
    }

public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapStart>(name => _players.Clear());
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        Logger.LogInformation("[DJ] v0.5.0 loaded");
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var userId = @event.Userid?.UserId ?? -1;
        _players.Remove(userId);
        return HookResult.Continue;
    }

    private void OnTick()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || player.IsBot || !player.PawnIsAlive)
                continue;

            var pawn = player.PlayerPawn?.Value;
            if (pawn == null)
                continue;

            var userId = player.UserId ?? -1;

            if (!_players.TryGetValue(userId, out var state))
            {
                state = new PlayerJumpState();
                _players[userId] = state;
            }

            var currentFlags = (PlayerFlags)pawn.Flags;
            var currentButtons = player.Buttons;

            bool wasOnGround = (state.PrevFlags & PlayerFlags.FL_ONGROUND) != 0;
            bool isOnGround = (currentFlags & PlayerFlags.FL_ONGROUND) != 0;
            bool wasJumping = (state.PrevButtons & PlayerButtons.Jump) != 0;
            bool isJumping = (currentButtons & PlayerButtons.Jump) != 0;

            // Force stamina to 0 every tick — removes landing speed penalty
            var movementServices = pawn.MovementServices;
            if (movementServices != null)
            {
                var moveService = new CCSPlayer_MovementServices(movementServices.Handle);
                moveService.Stamina = 0f;
            }

            // Bhop assist — track ticks since jump was PRESSED (not held)
            if (!isOnGround)
            {
                if (isJumping && !wasJumping)
                    state.TicksSinceAirJumpPress = 0;
                else
                    state.TicksSinceAirJumpPress++;
            }

            // Just landed — if they tapped jump recently enough, auto-jump
            if (isOnGround && !wasOnGround)
            {
                if (state.TicksSinceAirJumpPress <= Config.BhopWindowTicks)
                {
                    pawn.AbsVelocity.Z = Config.JumpVelocity;
                }
                state.TicksSinceAirJumpPress = 999;
            }

            // Reset jump count on ground
            if (isOnGround)
                state.JumpCount = 0;
            else if (state.JumpCount < 1)
                state.JumpCount = 1;

            // First jump horizontal boost — capped to prevent infinite acceleration
            float currentHSpeed = MathF.Sqrt(
                pawn.AbsVelocity.X * pawn.AbsVelocity.X +
                pawn.AbsVelocity.Y * pawn.AbsVelocity.Y);

            if (!isOnGround && wasOnGround && isJumping && currentHSpeed < 350f)
            {
                pawn.AbsVelocity.X *= Config.HorizontalBoost;
                pawn.AbsVelocity.Y *= Config.HorizontalBoost;
            }

            // Double jump — rising edge detection, airborne, under max jumps
            if (Config.JumpCount > 1
                && !wasJumping && isJumping && !wasOnGround && !isOnGround
                && state.JumpCount < Config.JumpCount)
            {
                state.JumpCount++;

                float djSpeed = MathF.Sqrt(
                    pawn.AbsVelocity.X * pawn.AbsVelocity.X +
                    pawn.AbsVelocity.Y * pawn.AbsVelocity.Y);
                float djBoost = djSpeed < 350f ? Config.HorizontalBoost : 1.0f;

                var newVelocity = new Vector(
                    pawn.AbsVelocity.X * djBoost,
                    pawn.AbsVelocity.Y * djBoost,
                    Config.JumpVelocity
                );
                pawn.Teleport(null, null, newVelocity);
            }

            state.PrevFlags = currentFlags;
            state.PrevButtons = currentButtons;
        }
    }
}

internal class PlayerJumpState
{
    public PlayerButtons PrevButtons { get; set; }
    public PlayerFlags PrevFlags { get; set; }
    public int JumpCount { get; set; }
    public int TicksSinceAirJumpPress { get; set; } = 999;
}