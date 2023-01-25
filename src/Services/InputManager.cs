using Raylib_CsLo;

namespace IslandGen.Services;

public class InputManager
{
    public void Update()
    {
        // Camera controls
        var gameCamera = ServiceManager.GetService<GameCamera>();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) gameCamera.ZoomOut();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) gameCamera.ZoomIn();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) gameCamera.PanUp();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) gameCamera.PanDown();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) gameCamera.PanLeft();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) gameCamera.PanRight();
    }
}