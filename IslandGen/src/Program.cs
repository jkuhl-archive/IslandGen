using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen;

internal static class Program
{    
    private static readonly Vector2 WindowSize = new(1000, 1000);
    
    public static void Main()
    {
        Raylib.InitWindow(WindowSize.X_int(), WindowSize.Y_int(), "Hello World");
        
        GameMap? gameMap = null;
        Vector2? mapStart = null;

        while (!Raylib.WindowShouldClose())
        {
            // If mouse is clicked generate a new map
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                gameMap = MapGeneration.GenerateMap();
                mapStart = new Vector2((WindowSize.X - gameMap.MapSize) / 2, (WindowSize.Y - gameMap.MapSize) / 2);
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw FPS and header
            Raylib.DrawFPS(0, 0);
            Raylib.DrawText("Click Left Mouse to generate map", 0, 20, 20, Raylib.WHITE);
            
            // Draw map
            if (gameMap != null && mapStart != null)
            {
                for (var mapX = 0; mapX < gameMap.MapSize; mapX++)
                for (var mapY = 0; mapY < gameMap.MapSize; mapY++)
                    Raylib.DrawPixelV(new Vector2(mapStart.Value.X + mapX, mapStart.Value.Y + mapY), gameMap.TileMap[mapX, mapY].GetTileColor());
            }
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}