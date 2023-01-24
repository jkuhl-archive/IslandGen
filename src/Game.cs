using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen;

internal static class Game
{
    private const int DefaultWindowWidth = 1280;
    private const int DefaultWindowHeight = 720;
    private const int TargetFrameRate = 60;

    private const string WindowName = "IslandGen";

    public static void Main()
    {
        // Initialize game window
        Raylib.InitWindow(DefaultWindowWidth, DefaultWindowHeight, WindowName);
        Raylib.SetTargetFPS(TargetFrameRate);

        // Initialize game services
        ServiceManager.AddService(new Random());
        ServiceManager.AddService(new GameCamera());
        ServiceManager.AddService(new GameMap());
        ServiceManager.AddService(new InputManager());
        ServiceManager.AddService(new GameUi());
        ServiceManager.AddService(new ScalingManager());
        ServiceManager.AddService(new TextureManager(TargetFrameRate));

        while (!Raylib.WindowShouldClose())
        {
            // Update
            ServiceManager.GetService<GameMap>().Update();
            ServiceManager.GetService<InputManager>().Update();
            ServiceManager.GetService<ScalingManager>().Update();
            ServiceManager.GetService<TextureManager>().Update();

            // Draw
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);
            ServiceManager.GetService<GameMap>().Draw();
            ServiceManager.GetService<GameUi>().Draw();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}