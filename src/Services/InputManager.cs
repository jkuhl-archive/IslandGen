using Raylib_CsLo;

namespace IslandGen.Services;

public class InputManager
{
    public void Update()
    {
        // Mouse controls
        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            ServiceManager.ReplaceService(new GameMap());

        // Camera controls
        var gameCamera = ServiceManager.GetService<GameCamera>();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) gameCamera.Camera2D.zoom -= .1f;
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) gameCamera.Camera2D.zoom += .1f;
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) gameCamera.Camera2D.target.X -= 100.0f;
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) gameCamera.Camera2D.target.X += 100.0f;
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) gameCamera.Camera2D.target.Y -= 100.0f;
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) gameCamera.Camera2D.target.Y += 100.0f;

        // Window controls
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_F1)) Raylib.ToggleFullscreen();
    }
}