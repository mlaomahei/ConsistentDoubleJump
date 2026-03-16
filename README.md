# ConsistentDoubleJump

A CounterStrikeSharp plugin for CS2 that adds configurable double jump with horizontal boost. Built for AWP servers.

## Features

- **Double Jump** — tap jump while airborne for a second jump. Requires releasing and re-pressing space, not holding.
- **Horizontal Boost** — both first and second jumps get a configurable speed multiplier so you feel propelled in your movement direction.
- **Speed Cap** — horizontal boost only applies under 350 u/s to prevent infinite acceleration through bhop chains.
- **Configurable** — jump count, velocity, and boost strength all adjustable via JSON config without rebuilding.

## Requirements

- [Metamod:Source 2.0](https://www.sourcemm.net/downloads.php/?branch=master)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) v1.0.363+
- .NET 8 Runtime (included in CSS "with-runtime" download)

## Installation

1. Clone the repo and build:
   ```bash
   git clone https://github.com/YOUR_USERNAME/ConsistentDoubleJump.git
   cd ConsistentDoubleJump
   dotnet build -c Release
   ```
2. Copy `bin/Release/net8.0/ConsistentDoubleJump.dll` and `ConsistentDoubleJump.deps.json` to:
   ```
   game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump/
   ```
3. Restart the server or load via CSS hot reload

## Configuration

After first load, a config file is generated at:
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
| `JumpCount` | `2` | Max jumps allowed (1 = normal, 2 = double, 3 = triple, etc.) |
| `JumpVelocity` | `300` | Vertical velocity applied on double jump |
| `HorizontalBoost` | `1.2` | Horizontal speed multiplier on jump (1.0 = no boost) |

## Recommended Server Config

For an AWP server, add these to `gamemode_casual_server.cfg`:

```
sv_cheats 1
sv_maxvelocity 3500
sv_maxspeed 215
sv_gravity 775
sv_airaccelerate 16
sv_accelerate 10
sv_autobunnyhopping 0
sv_enablebunnyhopping 1
sv_air_max_wishspeed 30

sv_staminalandcost 0
sv_staminajumpcost 0
sv_staminamax 0
sv_staminarecoveryrate 0
sv_accelerate_use_weapon_speed 0
sv_jump_spam_penalty_time 0
```

## Building

```bash
dotnet build -c Release
```

## WIP

This plugin is a work in progress. Planned features:
- `!dj` toggle command
- Permission system
- Bhop tuning
- AWP auto-give / weapon restrict

## Credits

- Built with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) by roflmuffin
- Movement research from [fidarit/cs2-DoubleJump](https://github.com/fidarit/cs2-DoubleJump)

## License

MIT