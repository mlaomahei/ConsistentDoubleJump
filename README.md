# ConsistentDoubleJump

A lightweight CounterStrikeSharp plugin for CS2 that adds configurable double jump with horizontal boost and live-tuning commands. Built for AWP and casual community servers.

## Features

- **Double jump** — release and re-press jump while airborne. Rising edge detection, not hold-to-activate.
- **Horizontal boost** — configurable speed multiplier on double jump. Capped at 350 u/s to prevent infinite acceleration.
- **Live-tuning commands** — adjust all settings from console without restarting. Tweak movement feel in real time.
- **Server ConVar commands** — force `sv_airaccelerate` and `sv_air_max_wishspeed` from the plugin, useful when workshop maps override your config.
- **Clean state management** — per-player tracking with automatic cleanup on disconnect and map change.

## Requirements

- [Metamod:Source 2.0](https://www.sourcemm.net/downloads.php/?branch=master)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) v1.0.363+
- .NET 8 Runtime (included in CSS "with-runtime" download)

## Installation

1. Clone and build:
   ```bash
   git clone https://github.com/mlaomahei/ConsistentDoubleJump.git
   cd ConsistentDoubleJump
   dotnet build -c Release
   ```
2. Copy `bin/Release/net8.0/ConsistentDoubleJump.dll` and `ConsistentDoubleJump.deps.json` to:
   ```
   game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump/
   ```
3. Restart the server or load via CSS hot reload.

## Configuration

Generated on first load at:
```
game/csgo/addons/counterstrikesharp/configs/plugins/ConsistentDoubleJump/ConsistentDoubleJump.json
```

```json
{
  "JumpCount": 2,
  "JumpVelocity": 290,
  "HorizontalBoost": 1.04,
  "ConfigVersion": 1
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `JumpCount` | `2` | Max jumps (1 = normal, 2 = double, 3 = triple, etc.) |
| `JumpVelocity` | `290` | Vertical velocity on double jump |
| `HorizontalBoost` | `1.04` | Horizontal speed multiplier on double jump (1.0 = no boost). Only applies under 350 u/s. |

## Console commands

All commands work from server console or in-game console. Without a value, they show the current setting.

| Command | Example | Description |
|---------|---------|-------------|
| `css_dj_vel` | `css_dj_vel 290` | Set/show double jump velocity |
| `css_dj_boost` | `css_dj_boost 1.04` | Set/show horizontal boost multiplier |
| `css_dj_jumps` | `css_dj_jumps 2` | Set/show max jump count |
| `css_dj_info` | `css_dj_info` | Show all current plugin settings |
| `css_dj_airaccel` | `css_dj_airaccel 200` | Force `sv_airaccelerate` (bypasses workshop map overrides) |
| `css_dj_wishspeed` | `css_dj_wishspeed 30` | Force `sv_air_max_wishspeed` (bypasses workshop map overrides) |

Changes from commands take effect instantly but don't persist across restarts. Update the JSON config to make values permanent.

## Recommended server config

For an AWP server, add these to `gamemode_casual_server.cfg`:

```
// Movement
sv_cheats 1
sv_maxvelocity 475
sv_maxspeed 215
sv_gravity 710
sv_airaccelerate 200
sv_accelerate 14
sv_autobunnyhopping 0
sv_enablebunnyhopping 1
sv_air_max_wishspeed 30

// Bhop timing (January 2026 movement system)
sv_bhop_time_window 0.0475
sv_jump_spam_penalty_time 0

// Weapon speed
sv_accelerate_use_weapon_speed 0

// Gameplay
mp_startmoney 16000
mp_afterroundmoney 16000
mp_friendlyfire 0
mp_autokick 0
sv_alltalk 1
```

Note: some workshop maps override ConVars like `sv_airaccelerate`. Use the plugin's `css_dj_airaccel` and `css_dj_wishspeed` commands to force values after map load.

### About the January 2026 movement update

As of the [January 22, 2026 CS2 update](https://www.counter-strike.net/news/updates), Valve completely reworked the movement system:

- **Stamina no longer affects jumping or landing.** ConVars like `sv_staminalandcost`, `sv_staminajumpcost`, `sv_staminamax`, and `sv_staminarecoveryrate` are now legacy and have no effect.
- **Landing speed penalty is now time-based**, not stamina-based.
- **Bhop is now engine-native.** `sv_bhop_time_window` controls how forgiving bhop timing is — any jump press within this window centered on landing time counts as a successful bhop. No plugin-level bhop assist needed.
- **`sv_jump_spam_penalty_time`** controls the cooldown between jump presses.
- **`sv_legacy_jump`** can restore old behavior on private servers if needed.

This means most community bhop assist plugins and stamina reset hacks are now obsolete. The engine handles it natively.

Full ConVar reference: [Valve Developer Wiki](https://developer.valvesoftware.com/wiki/List_of_Counter-Strike_2_console_commands_and_variables)

## Building

```bash
dotnet build -c Release
```

## Known limitations

- No animation plays on the second jump (engine-level, all double jump plugins have this)
- Landing sound plays slightly before actual ground contact (engine-level)
- Workshop maps can override server ConVars — use the plugin commands to force values

## WIP

Planned additions:
- `!dj` toggle command for players
- Permission system

For AWP server features like weapon restrict, auto-give AWP, and instant respawn, see dedicated community plugins:
- [Spawn-Loadout-GoldKingZ](https://github.com/oqyh/cs2-Spawn-Loadout-GoldKingZ) — auto-give weapons on spawn
- [WeaponRestrict](https://github.com/CS2Plugins/WeaponRestrict) — restrict weapon purchases
- [CS2-Deathmatch](https://github.com/NockyCZ/CS2-Deathmatch) — deathmatch with instant respawn

## Credits

- Built with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) by roflmuffin
- Movement research from [fidarit/cs2-DoubleJump](https://github.com/fidarit/cs2-DoubleJump) and the [CS2 modding community](https://cs2.poggu.me/)

## License

MIT
