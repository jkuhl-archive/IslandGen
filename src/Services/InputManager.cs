using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Services;

public class InputManager
{
    public void Update()
    {
        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive) return;

        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameMap = ServiceManager.GetService<GameMap>();
        var gameUi = ServiceManager.GetService<GameUi>();

        // Pause game
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_SPACE)) gameLogic.ToggleGamePaused();

        // Camera controls
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) gameLogic.GameCamera.ZoomOut();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) gameLogic.GameCamera.ZoomIn();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) gameLogic.GameCamera.PanUp();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) gameLogic.GameCamera.PanDown();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) gameLogic.GameCamera.PanLeft();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) gameLogic.GameCamera.PanRight();

        // Process mouse inputs if we are not mousing over the sidebar
        if (
            !gameUi.SidebarArea.PointInsideRectangle(Raylib.GetMousePosition()) &&
            !gameUi.CalendarArea.PointInsideRectangle(Raylib.GetMousePosition()) &&
            !(gameLogic.SelectedEntity != null &&
              gameUi.SelectedEntityMenuArea.PointInsideRectangle(Raylib.GetMousePosition()))
        )
            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                // Attempt to place the structure attached to the mouse
                if (gameLogic.MouseStructure != null)
                {
                    gameLogic.PlaceMouseStructure();
                    return;
                }

                // Unset selected entity
                gameLogic.SelectedEntity = null;

                // Attempt to select entity under mouse cursor
                foreach (var entity in gameLogic.GetAllEntities(true).Where(entity =>
                             entity.GetOccupiedTiles().Any(occupiedTile => occupiedTile == gameMap.GetMapMouseTile())))
                {
                    gameLogic.SelectedEntity = entity;
                    break;
                }
            }
    }
}