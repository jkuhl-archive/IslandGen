using System.Numerics;
using IslandGen.Extensions;
using IslandGen.Objects.ECS;
using Raylib_CsLo;

namespace IslandGen.Services;

public class InputManager
{
    private const float MouseCameraPanSpeed = 10;
    private const int MouseCameraPanThreshold = 5;

    public void Update()
    {
        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive) return;

        KeyboardInputs();
        MouseCameraInputs();
        MouseEntityInputs();
        MouseMiniMapInputs();
    }

    /// <summary>
    ///     Gets the mouse position within the game window.
    ///     Holding a mouse button down will cause Raylib's built in GetMousePosition() method
    ///     to return strange values if the mouse cursor leaves the game window.
    ///     This method always returns a value within the game window even when the cursor is not in the game window.
    /// </summary>
    /// <returns> Mouse position as a Vector2 </returns>
    public static Vector2 GetMousePosition()
    {
        var mousePosition = Raylib.GetMousePosition();
        var windowWidth = Raylib.GetRenderWidth();
        var windowHeight = Raylib.GetRenderHeight();

        if (mousePosition.X < 0)
            mousePosition.X = 0;
        else if (mousePosition.X > windowWidth) mousePosition.X = windowWidth;

        if (mousePosition.Y < 0)
            mousePosition.Y = 0;
        else if (mousePosition.Y > windowHeight) mousePosition.Y = windowHeight;

        return mousePosition;
    }

    /// <summary>
    ///     Processes keyboard input events
    /// </summary>
    private void KeyboardInputs()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();

        // Pause game
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_SPACE)) gameLogic.ToggleGamePaused();

        // Camera keyboard controls
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_UP)) gameLogic.GameCamera.ZoomOut();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_PAGE_DOWN)) gameLogic.GameCamera.ZoomIn();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) gameLogic.GameCamera.PanUp();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) gameLogic.GameCamera.PanDown();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_LEFT)) gameLogic.GameCamera.PanLeft();
        if (Raylib.IsKeyReleased(KeyboardKey.KEY_RIGHT)) gameLogic.GameCamera.PanRight();
    }

    /// <summary>
    ///     Processes camera mouse controls
    /// </summary>
    private void MouseCameraInputs()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var mousePosition = GetMousePosition();
        var mouseWheelState = Raylib.GetMouseWheelMove();
        var horizontalThreshold = scalingManager.WindowWidth / MouseCameraPanThreshold;
        var verticalThreshold = scalingManager.WindowHeight / MouseCameraPanThreshold;

        // Right mouse button held
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            // Handle horizontal panning if mouse cursor is close to window left or right edges
            if (mousePosition.X <= horizontalThreshold)
                gameLogic.GameCamera.PanLeft(MouseCameraPanSpeed);
            else if (mousePosition.X >= scalingManager.WindowWidth - horizontalThreshold)
                gameLogic.GameCamera.PanRight(MouseCameraPanSpeed);

            // Handle vertical panning if mouse cursor is close to window top or bottom edges
            if (mousePosition.Y <= verticalThreshold)
                gameLogic.GameCamera.PanUp(MouseCameraPanSpeed);
            else if (mousePosition.Y >= scalingManager.WindowHeight - verticalThreshold)
                gameLogic.GameCamera.PanDown(MouseCameraPanSpeed);
        }

        // If mouse wheel has been scrolled, adjust camera zoom
        switch (mouseWheelState)
        {
            case 1:
                gameLogic.GameCamera.ZoomIn();
                break;
            case -1:
                gameLogic.GameCamera.ZoomOut();
                break;
        }
    }

    /// <summary>
    ///     Processes mouse inputs that interact with entities
    /// </summary>
    private void MouseEntityInputs()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameUi = ServiceManager.GetService<GameUi>();
        var mousePosition = GetMousePosition();

        // Return if mouse position is over a UI element
        if (
            gameUi.CalendarArea.PointInsideRectangle(mousePosition) ||
            gameUi.SidebarArea.PointInsideRectangle(mousePosition) ||
            gameUi.SpeedControlsArea.PointInsideRectangle(mousePosition) ||
            (gameLogic.SelectedEntity != null && EntityBase.SelectedEntityMenuArea.PointInsideRectangle(mousePosition))
        ) return;

        // Left mouse click
        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            // Attempt to place the structure attached to the mouse
            if (gameLogic.MouseStructure != null)
            {
                gameLogic.PlaceMouseStructure();
                return;
            }

            // Unset previously selected entity
            gameLogic.UnsetSelectedEntity();

            // Attempt to select entity under mouse cursor
            foreach (var entity in gameLogic.GetAllEntities(true).Where(entity =>
                         entity.GetOccupiedTiles()
                             .Any(occupiedTile => occupiedTile == gameLogic.GameMap.GetMapMouseTile())))
            {
                gameLogic.SetSelectedEntity(entity);
                break;
            }
        }
    }

    /// <summary>
    ///     Processes mouse inputs that interact with the mini map
    /// </summary>
    private void MouseMiniMapInputs()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameUi = ServiceManager.GetService<GameUi>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var mousePosition = GetMousePosition();

        if (!gameUi.MiniMapArea.PointInsideRectangle(mousePosition)) return;

        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        {
            var miniMapPosition = (mousePosition - gameUi.MiniMapArea.Start()) / scalingManager.ScaleFactor;
            gameLogic.GameCamera.LookAtTile((miniMapPosition.X_int(), miniMapPosition.Y_int()));
        }
    }
}