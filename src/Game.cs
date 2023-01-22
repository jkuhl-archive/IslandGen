using System.Numerics;
using IslandGen.Data.ECS.Entities;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen;

internal static class Game
{
    private const int DefaultWindowWidth = 1280;
    private const int DefaultWindowHeight = 720;
    private const int DefaultTargetFrameRate = 60;

    private const string WindowName = "IslandGen";

    public static void Main()
    {
        // Initialize game window
        Raylib.InitWindow(DefaultWindowWidth, DefaultWindowHeight, WindowName);
        Raylib.SetTargetFPS(DefaultTargetFrameRate);

        // Initialize game services
        ServiceManager.AddService(new Random());
        ServiceManager.AddService(new EntityManager());
        ServiceManager.AddService(new GameCamera());
        ServiceManager.AddService(new GameMap());
        ServiceManager.AddService(new InputManager());
        ServiceManager.AddService(new GameUi());
        ServiceManager.AddService(new TextureManager());

        // Spawn some basic entities for testing
        // TODO: Remove this once we have a better way to spawn entities
        ServiceManager.GetService<EntityManager>().Entities.Add(new Colonist("Bob", new Vector2(500, 500)));

        while (!Raylib.WindowShouldClose())
        {
            // Update
            ServiceManager.GetService<InputManager>().Update();
            ServiceManager.GetService<EntityManager>().Update();

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