using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen;

internal static class Program
{
    private const int MapSize = 100;
    private const int TargetFrameRate = 60;
    private const int TileTextureSize = 16;
    private const int UiPaddingSize = 5;

    private const float MiniMapScale = 2.0f;

    private const string TexturesDirectory = "assets/textures";
    private const string WindowName = "IslandGen";
    
    private static readonly Vector2 WindowSize = new(1280, 720);
    private static readonly Vector2 MapRenderTextureSize = new(MapSize * TileTextureSize, MapSize * TileTextureSize);
    private static readonly Vector2 MiniMapTextureSize = new(MapSize, MapSize);

    private static readonly Vector2 MiniMapStart = new(
        WindowSize.X - MiniMapTextureSize.X * MiniMapScale - UiPaddingSize * 2,
        WindowSize.Y - MiniMapTextureSize.Y * MiniMapScale - UiPaddingSize * 2);

    private static GameMap _gameMap = MapGeneration.GenerateMap(MapSize);

    /// <summary>
    ///     Loads all textures within the TexturesDirectory and returns a dictionary containing them
    /// </summary>
    /// <returns> Dictionary that stores texture names as keys and textures as values </returns>
    private static Dictionary<string, Texture> LoadTextureDictionary()
    {
        var textureDirectory = new DirectoryInfo(TexturesDirectory);
        var textureFileList = textureDirectory.GetFiles();
        return textureFileList.ToDictionary(textureFile => Path.ChangeExtension(textureFile.Name, null),
            textureFile => Raylib.LoadTexture(textureFile.FullName));
    }

    public static void Main()
    {
        // Initialize game window
        Raylib.InitWindow(WindowSize.X_int(), WindowSize.Y_int(), WindowName);
        Raylib.SetTargetFPS(TargetFrameRate);

        // Initialize map render camera 
        Camera2D mapRenderCamera = new()
        {
            zoom = 0.8f
        };

        // Initialize a textures to render the game map and minimap to
        var gameMapTexture = new RenderTexturePro(MapRenderTextureSize);
        var miniMapTexture = new RenderTexturePro(MiniMapTextureSize)
        {
            Scale = MiniMapScale
        };
        miniMapTexture.DestinationRectangle.X = MiniMapStart.X + UiPaddingSize;
        miniMapTexture.DestinationRectangle.Y = MiniMapStart.Y + UiPaddingSize;

        // Load textures
        var textureDictionary = LoadTextureDictionary();

        while (!Raylib.WindowShouldClose())
        {
            // Handle input
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                _gameMap = MapGeneration.GenerateMap(MapSize);
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) mapRenderCamera.zoom -= .1f;
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) mapRenderCamera.zoom += .1f;
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) mapRenderCamera.target.X -= 100.0f;
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) mapRenderCamera.target.X += 100.0f;
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) mapRenderCamera.target.Y -= 100.0f;
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) mapRenderCamera.target.Y += 100.0f;

            // Render map to texture
            Raylib.BeginTextureMode(gameMapTexture.RenderTexture);
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.BeginMode2D(mapRenderCamera);
            for (var mapX = 0; mapX < _gameMap.MapSize; mapX++)
            for (var mapY = 0; mapY < _gameMap.MapSize; mapY++)
            {
                var texture = textureDictionary[_gameMap.TileMap[mapX, mapY].GetTileTextureName()];
                Raylib.DrawTexture(texture, mapX * TileTextureSize, mapY * TileTextureSize, Raylib.WHITE);
            }
            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // Render mini map to texture
            Raylib.BeginTextureMode(miniMapTexture.RenderTexture);
            Raylib.ClearBackground(Raylib.BLACK);
            for (var mapX = 0; mapX < _gameMap.MapSize; mapX++)
            for (var mapY = 0; mapY < _gameMap.MapSize; mapY++)
                Raylib.DrawPixelV(new Vector2(mapX, mapY), _gameMap.TileMap[mapX, mapY].GetTileColor());
            Raylib.EndTextureMode();

            // Start drawing 
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw game map texture
            gameMapTexture.Draw();

            // Draw minimap
            Raylib.DrawRectangle(MiniMapStart.X_int(), MiniMapStart.Y_int(),
                (int)(MiniMapTextureSize.X * MiniMapScale + UiPaddingSize * 2),
                (int)(MiniMapTextureSize.Y * MiniMapScale + UiPaddingSize * 2), Raylib.WHITE);
            miniMapTexture.Draw();

            // Draw header
            Raylib.DrawRectangle(0, 0, 460, 110, Raylib.WHITE);
            Raylib.DrawRectangle(5, 5, 450, 100, Raylib.BLACK);
            Raylib.DrawFPS(10, 10);
            Raylib.DrawText("Click Left Mouse to generate a new map", 10, 30, 20, Raylib.WHITE);
            Raylib.DrawText("PageUp / PageDown to zoom", 10, 50, 20, Raylib.WHITE);
            Raylib.DrawText("Arrow Keys move map", 10, 70, 20, Raylib.WHITE);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}