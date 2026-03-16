# ConsistentDoubleJump

A CounterStrikeSharp plugin for CS2 that adds configurable double jump, horizontal boost, bhop assist, and per-tick stamina reset.

Built for AWP and casual servers that want snappy, responsive movement without full bhop/surf physics.

## Features

- **Double Jump** — tap jump while airborne for a second jump. Requires releasing and re-pressing, not holding space.
- **Horizontal Boost** — both jumps get a configurable speed multiplier so you actually feel propelled in the direction you're moving.
- **Speed Cap** — horizontal boost only applies under 350 u/s to prevent infinite acceleration through bhop chains.
- **Bhop Assist** — forgiving jump window. If you tap jump within a configurable number of ticks before landing, it auto-jumps on contact. Not automatic — you still need to time a press.
- **Stamina Reset** — forces player stamina to 0 every tick, reducing the landing speed penalty CS2 applies after hard falls.

## Requirements

- [Metamod:Source 2.0](https://www.sourcemm.net/downloads.php/?branch=master)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) v1.0.363+
- .NET 8 Runtime (included in CSS "with-runtime" download)

## Installation

1. Clone the repo and build:
```bash
   git clone https://github.com/mlaomahei/ConsistentDoubleJump.git
   cd ConsistentDoubleJump
   dotnet build -c Release
```
2. Copy the `bin/Release/net8.0/ConsistentDoubleJump.dll` (and `.deps.json`) to:
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
  "JumpVelocity": 280,
  "HorizontalBoost": 1.2,
  "BhopWindowTicks": 8,
  "ConfigVersion": 1
}
```

| Setting | Default | Description |
|---------|---------|-------------|
| `JumpCount` | `2` | Max jumps allowed (1 = normal, 2 = double, 3 = triple, etc.) |
| `JumpVelocity` | `280` | Vertical velocity applied on double jump |
| `HorizontalBoost` | `1.2` | Horizontal speed multiplier on jump (1.0 = no boost) |
| `BhopWindowTicks` | `8` | How many ticks before landing a jump press counts for bhop assist. Higher = more forgiving. 0 = disabled. |

## Recommended Server ConVars

For an AWP server, pair this plugin with these ConVars in your `gamemode_casual_server.cfg`:

```
sv_cheats 1
sv_maxvelocity 3500
sv_maxspeed 280
sv_airaccelerate 25
sv_accelerate 10
sv_autobunnyhopping 0
sv_enablebunnyhopping 1
sv_air_max_wishspeed 40

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

Output DLL will be in `bin/Release/net8.0/`.

## Credits

- Built with [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) by roflmuffin
- Movement research from [fidarit/cs2-DoubleJump](https://github.com/fidarit/cs2-DoubleJump) and the CS2 modding community

## License

MIT