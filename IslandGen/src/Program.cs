using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen;

internal static class Program
{
    private const int MapSize = 100;
    private const int TargetFrameRate = 60;

    private static readonly Vector2 WindowSize = new(800, 600);
    private static readonly Vector2 VirtualScreenSize = new(WindowSize.X / 4, WindowSize.Y / 4);

    private static readonly Vector2 VirtualScreenRatio =
        new(WindowSize.X / VirtualScreenSize.X, WindowSize.Y / VirtualScreenSize.Y);

    private static readonly Vector2 MapDrawingStart =
        new((VirtualScreenSize.X - MapSize) / 2, (VirtualScreenSize.Y - MapSize) / 2);

    private static GameMap _gameMap = new(MapSize);

    public static void Main()
    {
        Raylib.InitWindow(WindowSize.X_int(), WindowSize.Y_int(), "Hello World");
        Raylib.SetTargetFPS(TargetFrameRate);

        Camera2D renderCamera = new();
        Camera2D screenSpaceCamera = new();
        var renderTexture = Raylib.LoadRenderTexture(VirtualScreenSize.X_int(), VirtualScreenSize.Y_int());
        Rectangle renderSourceRectangle =
            new(0.0f, 0.0f, renderTexture.texture.width,
                -renderTexture.texture.height); // The source rectangle's height is flipped due to OpenGL reasons
        Rectangle renderDestinationRectangle = new(-VirtualScreenRatio.X, -VirtualScreenRatio.X,
            WindowSize.X + VirtualScreenRatio.X * 2, WindowSize.Y + VirtualScreenRatio.X * 2);

        renderCamera.zoom = 1.0f;
        screenSpaceCamera.zoom = 1.0f;

        while (!Raylib.WindowShouldClose())
        {
            // If mouse is clicked generate a new map
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                _gameMap = new GameMap(MapSize);
                MapGeneration.GenerateMap(_gameMap);
            }

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) renderCamera.zoom -= .1f;

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) renderCamera.zoom += .1f;

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) renderCamera.target.X -= 10.0f;

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) renderCamera.target.X += 10.0f;

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) renderCamera.target.Y -= 10.0f;

            if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) renderCamera.target.Y += 10.0f;

            // Render map to a texture
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.BeginMode2D(renderCamera);
            for (var mapX = 0; mapX < _gameMap.MapSize; mapX++)
            for (var mapY = 0; mapY < _gameMap.MapSize; mapY++)
                Raylib.DrawPixelV(new Vector2(MapDrawingStart.X + mapX, MapDrawingStart.Y + mapY),
                    _gameMap.TileMap[mapX, mapY].GetTileColor());
            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // Start drawing 
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw rendered texture to camera
            Raylib.BeginMode2D(screenSpaceCamera);
            Raylib.DrawTexturePro(renderTexture.texture, renderSourceRectangle, renderDestinationRectangle,
                Vector2.Zero, 0.0f, Raylib.WHITE);
            Raylib.EndMode2D();

            // Draw FPS and header
            Raylib.DrawFPS(0, 0);
            Raylib.DrawText("Click Left Mouse to generate map", 0, 20, 20, Raylib.WHITE);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}