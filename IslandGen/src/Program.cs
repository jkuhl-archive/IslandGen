using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen;

internal static class Program
{
    private const int MapSize = 200;

    private static readonly Vector2 WindowSize = new(1000, 1000);
    private static readonly Vector2 MapDrawingStart = new((WindowSize.X - MapSize) / 2, (WindowSize.Y - MapSize) / 2);

    private static GameMap _gameMap = new(MapSize);

    public static void Main()
    {
        Raylib.InitWindow(WindowSize.X_int(), WindowSize.Y_int(), "Hello World");

        while (!Raylib.WindowShouldClose())
        {
            // If mouse is clicked generate a new map
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                _gameMap = new GameMap(MapSize);
                MapGeneration.GenerateMap(_gameMap);
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw FPS and header
            Raylib.DrawFPS(0, 0);
            Raylib.DrawText("Click Left Mouse to generate map", 0, 20, 20, Raylib.WHITE);

            // Draw map
            for (var mapX = 0; mapX < _gameMap.MapSize; mapX++)
            for (var mapY = 0; mapY < _gameMap.MapSize; mapY++)
                Raylib.DrawPixelV(new Vector2(MapDrawingStart.X + mapX, MapDrawingStart.Y + mapY),
                    _gameMap.TileMap[mapX, mapY].GetTileColor());
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}