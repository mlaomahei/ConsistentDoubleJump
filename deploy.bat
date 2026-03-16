@echo off
echo [BUILD] Building ConsistentDoubleJump...
dotnet build -c Release

if %ERRORLEVEL% NEQ 0 (
    echo [BUILD] Build failed!
    pause
    exit /b 1
)

echo [DEPLOY] Copying to CS2 server...

REM Create plugin directory if it doesn't exist
docker exec cs2-dev mkdir -p /home/steam/cs2-dedicated/game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump

REM Copy the built DLL and deps
docker cp bin\Release\net8.0\ConsistentDoubleJump.dll cs2-dev:/home/steam/cs2-dedicated/game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump/
docker cp bin\Release\net8.0\ConsistentDoubleJump.deps.json cs2-dev:/home/steam/cs2-dedicated/game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump/
docker cp bin\Release\net8.0\ConsistentDoubleJump.pdb cs2-dev:/home/steam/cs2-dedicated/game/csgo/addons/counterstrikesharp/plugins/ConsistentDoubleJump/

echo [DEPLOY] Done! CounterStrikeSharp will hot-reload the plugin automatically.
echo [DEPLOY] If it doesn't, restart the server with: docker compose restart cs2
