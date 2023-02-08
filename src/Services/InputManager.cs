using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Services;

public class InputManager
{
    public void Update()
    {
        var gameCamera = ServiceManager.GetService<GameCamera>();
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameUi = ServiceManager.GetService<GameUi>();

        // Camera controls
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) gameCamera.ZoomOut();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) gameCamera.ZoomIn();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) gameCamera.PanUp();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) gameCamera.PanDown();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) gameCamera.PanLeft();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) gameCamera.PanRight();

        // Process mouse inputs if we are not mousing over the sidebar
        if (!gameUi.SidebarArea.PointInsideRectangle(Raylib.GetMousePosition()))
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                gameLogic.PlaceMouseStructure();
    }
}