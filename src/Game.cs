using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Services;
using IslandGen.Utils;
using Raylib_CsLo;

namespace IslandGen;

internal static class Game
{
    private const int DefaultWindowWidth = 1280;
    private const int DefaultWindowHeight = 720;
    private const string WindowName = "IslandGen";

    public static void Main()
    {
        // Initialize game window
        Raylib.InitWindow(DefaultWindowWidth, DefaultWindowHeight, WindowName);
        Raylib.InitAudioDevice();

        // Load game settings
        ServiceManager.AddService(SaveUtils.LoadSettings());
        ServiceManager.GetService<GameSettings>().ApplySettings();

        // Initialize core game services
        ServiceManager.AddService(new GameSettingsUi());
        ServiceManager.AddService(new GameUi());
        ServiceManager.AddService(new InputManager());
        ServiceManager.AddService(new MainMenuUi());
        ServiceManager.AddService(new Random());
        ServiceManager.AddService(new ScalingManager());
        ServiceManager.AddService(new StateManager());

        while (!Raylib.WindowShouldClose())
        {
            // Update scaling
            ServiceManager.GetService<ScalingManager>().Update();

            // Start drawing
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            switch (ServiceManager.GetService<StateManager>().GameState)
            {
                case GameState.MainMenu:
                    // Draw Main Menu
                    ServiceManager.GetService<MainMenuUi>().Draw();
                    break;

                case GameState.InGame:
                    // Update Game
                    Assets.Update();
                    ServiceManager.GetService<InputManager>().Update();
                    ServiceManager.GetService<GameLogic>().Update();
                    ServiceManager.GetService<GameUi>().Update();

                    // Draw Game
                    ServiceManager.GetService<GameMap>().Draw();
                    ServiceManager.GetService<GameUi>().Draw();
                    break;
            }

            // Draw settings menu overlay if enabled
            ServiceManager.GetService<GameSettingsUi>().Draw();

            // End drawing
            Raylib.EndDrawing();
        }

        Assets.UnloadAssets();
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}