# ConsistentDoubleJump

A lightweight CounterStrikeSharp plugin for CS2 that adds configurable double jump with horizontal boost. Built for AWP and casual community servers.

## Features

- **Double jump** — release and re-press jump while airborne. Rising edge detection, not hold-to-activate.
- **Horizontal boost** — configurable speed multiplier on double jump so you feel propelled forward, not just straight up. Capped at 350 u/s to prevent infinite acceleration.
- **Configurable** — jump count, velocity, and boost strength all adjustable via JSON config. No rebuild needed.
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
  "JumpVelocity": 300,
  "HorizontalBoost": 1.2,
  "ConfigVersion": 1
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `JumpCount` | `2` | Max jumps (1 = normal, 2 = double, 3 = triple, etc.) |
| `JumpVelocity` | `300` | Vertical velocity on double jump |
| `HorizontalBoost` | `1.2` | Horizontal speed multiplier on double jump (1.0 = no boost). Only applies under 350 u/s. |

## Recommended server config

For an AWP server, add these to `gamemode_casual_server.cfg`:

```
// Movement
sv_cheats 1
sv_maxvelocity 3500
sv_maxspeed 215
sv_gravity 725
sv_airaccelerate 25
sv_accelerate 10
sv_autobunnyhopping 0
sv_enablebunnyhopping 1
sv_air_max_wishspeed 45

// Bhop timing (January 2026 movement system)
sv_bhop_time_window 0.03
sv_jump_spam_penalty_time 0

// Weapon speed
sv_accelerate_use_weapon_speed 0
```

### A note on the January 2026 movement update

As of the [January 22, 2026 CS2 update](https://www.counter-strike.net/news/updates), Valve completely reworked the movement system:

- **Stamina no longer affects jumping or landing.** ConVars like `sv_staminalandcost`, `sv_staminajumpcost`, `sv_staminamax`, and `sv_staminarecoveryrate` are now legacy and have no effect.
- **Landing speed penalty is now time-based**, not stamina-based.
- **Bhop is now engine-native.** `sv_bhop_time_window` controls how forgiving bhop timing is — any jump press within this window centered on landing time counts as a successful bhop. No plugin-level bhop assist needed.
- **`sv_jump_spam_penalty_time`** controls the cooldown between jump presses.
- **`sv_legacy_jump`** can restore old behavior on private servers if needed.

This means most community bhop assist plugins and stamina reset hacks are now obsolete. The engine handles it natively — just tune the two ConVars above.

Full ConVar reference: [Valve Developer Wiki](https://developer.valvesoftware.com/wiki/List_of_Counter-Strike_2_console_commands_and_variables)

## Building

```bash
dotnet build -c Release
```

## WIP

This plugin is a work in progress. The double jump and movement tuning are functional. Planned additions:
- `!dj` toggle command
- Permission system

For AWP server features like weapon restrict, auto-give AWP, and instant respawn, check out dedicated community plugins:
- [Spawn-Loadout-GoldKingZ](https://github.com/oqyh/cs2-Spawn-Loadout-GoldKingZ) — auto-give weapons on spawn
- [WeaponRestrict](https://github.com/CS2Plugins/WeaponRestrict) — restrict weapon purchases
- [CS2-Deathmatch](https://github.com/NockyCZ/CS2-Deathmatch) — deathmatch with instant respawn

## Credits

- Built with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) by roflmuffin
- Movement research from [fidarit/cs2-DoubleJump](https://github.com/fidarit/cs2-DoubleJump) and the [CS2 modding community](https://cs2.poggu.me/)

## License

MIT