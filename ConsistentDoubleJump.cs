using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace ConsistentDoubleJump;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("JumpCount")]
    public int JumpCount { get; set; } = 2;

    [JsonPropertyName("JumpVelocity")]
    public float JumpVelocity { get; set; } = 290.0f;

    [JsonPropertyName("HorizontalBoost")]
    public float HorizontalBoost { get; set; } = 1.04f;
}

public class ConsistentDoubleJump : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "ConsistentDoubleJump";
    public override string ModuleVersion => "1.1.0";
    public override string ModuleAuthor => "Vqesh";
    public override string ModuleDescription => "Double jump for CS2";

    public PluginConfig Config { get; set; } = new();

    private readonly Dictionary<int, PlayerJumpState> _players = new();

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
        Logger.LogInformation("[DJ] Config: JumpCount={Count} Velocity={Vel} HBoost={Boost}",
            config.JumpCount, config.JumpVelocity, config.HorizontalBoost);
    }

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapStart>(name => _players.Clear());
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        Logger.LogInformation("[DJ] v1.1.0 loaded");
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var userId = @event.Userid?.UserId ?? -1;
        _players.Remove(userId);
        return HookResult.Continue;
    }

    [ConsoleCommand("css_dj_vel", "Set double jump velocity")]
    [CommandHelper(minArgs: 0, usage: "[value]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnVelCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[DJ] JumpVelocity = {Config.JumpVelocity}");
            return;
        }

        if (float.TryParse(command.ArgByIndex(1), out float val))
        {
            Config.JumpVelocity = val;
            command.ReplyToCommand($"[DJ] JumpVelocity set to {val}");
        }
    }

    [ConsoleCommand("css_dj_boost", "Set horizontal boost multiplier")]
    [CommandHelper(minArgs: 0, usage: "[value]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnBoostCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[DJ] HorizontalBoost = {Config.HorizontalBoost}");
            return;
        }

        if (float.TryParse(command.ArgByIndex(1), out float val))
        {
            Config.HorizontalBoost = val;
            command.ReplyToCommand($"[DJ] HorizontalBoost set to {val}");
        }
    }

    [ConsoleCommand("css_dj_jumps", "Set max jump count")]
    [CommandHelper(minArgs: 0, usage: "[value]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnJumpsCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[DJ] JumpCount = {Config.JumpCount}");
            return;
        }

        if (int.TryParse(command.ArgByIndex(1), out int val))
        {
            Config.JumpCount = val;
            command.ReplyToCommand($"[DJ] JumpCount set to {val}");
        }
    }

    [ConsoleCommand("css_dj_info", "Show all current settings")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnInfoCommand(CCSPlayerController? player, CommandInfo command)
    {
        command.ReplyToCommand($"[DJ] JumpCount={Config.JumpCount} JumpVelocity={Config.JumpVelocity} HorizontalBoost={Config.HorizontalBoost}");
    }

    [ConsoleCommand("css_dj_airaccel", "Set sv_airaccelerate")]
    [CommandHelper(minArgs: 0, usage: "[value]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnAirAccelCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand("[DJ] Use: css_dj_airaccel <value>");
            return;
        }

        Server.ExecuteCommand($"sv_airaccelerate {command.ArgByIndex(1)}");
        command.ReplyToCommand($"[DJ] sv_airaccelerate set to {command.ArgByIndex(1)}");
    }

    [ConsoleCommand("css_dj_wishspeed", "Set sv_air_max_wishspeed")]
    [CommandHelper(minArgs: 0, usage: "[value]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnWishSpeedCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand("[DJ] Use: css_dj_wishspeed <value>");
            return;
        }

        Server.ExecuteCommand($"sv_air_max_wishspeed {command.ArgByIndex(1)}");
        command.ReplyToCommand($"[DJ] sv_air_max_wishspeed set to {command.ArgByIndex(1)}");
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

            // Reset jump count on ground
            if (isOnGround)
                state.JumpCount = 0;
            else if (state.JumpCount < 1)
                state.JumpCount = 1;

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
}