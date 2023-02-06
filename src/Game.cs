using IslandGen.Data.Enum;
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
        ServiceManager.AddService(new GameCamera());
        ServiceManager.AddService(new GameUi());
        ServiceManager.AddService(new InputManager());
        ServiceManager.AddService(new MainMenuUi());
        ServiceManager.AddService(new Random());
        ServiceManager.AddService(new ScalingManager());
        ServiceManager.AddService(new StateManager());
        ServiceManager.AddService(new TextureManager(TargetFrameRate));

        while (!Raylib.WindowShouldClose())
        {
            // Update scaling
            ServiceManager.GetService<ScalingManager>().Update();

            switch (ServiceManager.GetService<StateManager>().GameState)
            {
                case GameState.MainMenu:
                    // Update Main Menu
                    ServiceManager.GetService<MainMenuUi>().Update();

                    // Draw Main Menu
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Raylib.BLACK);
                    ServiceManager.GetService<MainMenuUi>().Draw();
                    Raylib.EndDrawing();
                    break;

                case GameState.InGame:
                    // Update Game
                    ServiceManager.GetService<InputManager>().Update();
                    ServiceManager.GetService<TextureManager>().Update();
                    ServiceManager.GetService<GameLogic>().Update();
                    ServiceManager.GetService<GameUi>().Update();

                    // Draw Game
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Raylib.BLACK);
                    ServiceManager.GetService<GameMap>().Draw();
                    ServiceManager.GetService<GameUi>().Draw();
                    Raylib.EndDrawing();
                    break;
            }
        }

        Raylib.CloseWindow();
    }
}