using System.Numerics;
using IslandGen.Data.ECS.Entities.Structures;
using IslandGen.Data.Enum;
using IslandGen.Data.Textures;
using IslandGen.Extensions;
using IslandGen.UI;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameUi
{
    private const int ButtonsWidth = 100;
    private const int ButtonsHeight = 100;
    private const int ButtonWidth = 50;
    private const int ButtonHeight = 20;
    private const int ButtonsRows = ButtonsWidth / ButtonWidth;
    private const int ButtonsColumns = ButtonsHeight / ButtonHeight;
    private const int CalendarWidth = 115;
    private const int CalendarHeight = 17;
    private const int CalendarWidthPaddingSegments = 2;
    private const int CalendarHeightPaddingSegments = 3;
    private const int MiniMapWidth = 100;
    private const int MiniMapHeight = 100;
    private const int SidebarWidth = 100;
    private const int SidebarHeight = 200;
    private const int SidebarWidthPaddingSegments = 2;
    private const int SidebarHeightPaddingSegments = 3;

    private readonly List<Button> _buttonsList;
    private readonly RenderTexturePro _miniMapTexture;

    private Rectangle _buttonsArea;
    private string _calendarString;
    private string _debugInfo;
    private Rectangle _miniMapArea;

    /// <summary>
    ///     Service that manages the game's UI
    /// </summary>
    public GameUi()
    {
        _buttonsList = new List<Button>
        {
            new("Zoom In", () => ServiceManager.GetService<GameCamera>().ZoomIn()),
            new("Zoom Out", () => ServiceManager.GetService<GameCamera>().ZoomOut()),
            new("Save Island", StateManager.SaveGame),
            new("Load Island", StateManager.LoadGame),
            new("New Island", StateManager.NewGame),
            new("Change Speed", () => ServiceManager.GetService<GameLogic>().ChangeSpeed()),
            new("Debug Mode",
                () => ServiceManager.GetService<GameSettings>().DebugMode =
                    !ServiceManager.GetService<GameSettings>().DebugMode),
            new("Fullscreen", Raylib.ToggleFullscreen),
            new("New Shelter", () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Shelter())),
            new("Main Menu", ReturnToMainMenu)
        };

        _calendarString = string.Empty;
        _debugInfo = string.Empty;
        _miniMapTexture = new RenderTexturePro(new Vector2(MiniMapWidth, MiniMapHeight));
    }

    public Rectangle CalendarArea { get; private set; }
    public Rectangle SidebarArea { get; private set; }

    public void Draw()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameMap = ServiceManager.GetService<GameMap>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Render map to minimap texture
        Raylib.BeginTextureMode(_miniMapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        for (var mapX = 0; mapX < gameMap.GetMapSize(); mapX++)
        for (var mapY = 0; mapY < gameMap.GetMapSize(); mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), gameMap.TileMap[mapX, mapY].GetTileColor());

        // Render entities to minimap texture
        foreach (var entity in gameLogic.Colonists)
            Raylib.DrawPixelV(new Vector2(entity.GetMapPosition().Item1, entity.GetMapPosition().Item2), Raylib.BLACK);
        Raylib.DrawRectangleLinesEx(gameMap.GetVisibleMapArea(), 1, Raylib.RED);
        Raylib.EndTextureMode();

        // Draw calendar popup
        DrawPopUp(_calendarString, CalendarArea);

        // Draw sidebar backdrop
        Raylib.DrawRectangleRec(SidebarArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(_buttonsArea, Raylib.GRAY);

        // Draw buttons
        foreach (var button in _buttonsList) button.Draw();

        // Draw minimap
        _miniMapTexture.Draw();

        // Draw debug info
        if (gameSettings.DebugMode) DrawPopUp(_debugInfo, new Rectangle(), true);
    }

    public void Update()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Set calendar string
        _calendarString = $"{gameLogic.CurrentDateTime.ToLongDateString()}\n" +
                          $"{gameLogic.CurrentDateTime.ToLongTimeString()}";

        // Set debug info
        if (gameSettings.DebugMode)
        {
            var gameCamera = ServiceManager.GetService<GameCamera>();
            var gameMap = ServiceManager.GetService<GameMap>();
            var scalingManager = ServiceManager.GetService<ScalingManager>();

            _debugInfo =
                $"FPS: {Raylib.GetFPS()}\n" +
                $"Window Resolution: {scalingManager.WindowWidth}x{scalingManager.WindowHeight}\n" +
                $"Scaling Factor: {scalingManager.ScaleFactor}\n" +
                $"Game Speed: {gameLogic.GameSpeed} ({gameLogic.GameSpeed.GetSpeedMultiplier()}x)\n" +
                "\n" +
                $"Mouse Window Position: {Raylib.GetMousePosition()}\n" +
                $"Mouse Map Position: {gameMap.GetMapMousePosition()}\n" +
                $"Mouse Highlighted Tile: {gameMap.GetMapMouseTile()}\n" +
                "\n" +
                $"Camera Zoom: {gameCamera.Camera.zoom}x\n" +
                $"Camera Position: {gameCamera.Camera.target}\n" +
                $"Camera Visible Map Tiles: {gameMap.GetVisibleMapArea().String()}";
        }
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var calendarWidthPadding = scalingManager.Padding * CalendarWidthPaddingSegments;
        var calendarHeightPadding = scalingManager.Padding * CalendarHeightPaddingSegments;
        var sidebarWidthPadding = scalingManager.Padding * SidebarWidthPaddingSegments;
        var sidebarHeightPadding = scalingManager.Padding * SidebarHeightPaddingSegments;

        // Set calendar area
        CalendarArea = new Rectangle(
            scalingManager.WindowWidth - CalendarWidth * scalingManager.ScaleFactor - calendarWidthPadding,
            0,
            (int)(CalendarWidth * scalingManager.ScaleFactor + calendarWidthPadding),
            (int)(CalendarHeight * scalingManager.ScaleFactor + calendarHeightPadding));

        // Set sidebar area
        SidebarArea = new Rectangle(
            scalingManager.WindowWidth - SidebarWidth * scalingManager.ScaleFactor - sidebarWidthPadding,
            scalingManager.WindowHeight - SidebarHeight * scalingManager.ScaleFactor - sidebarHeightPadding,
            (int)(SidebarWidth * scalingManager.ScaleFactor + sidebarWidthPadding),
            (int)(SidebarHeight * scalingManager.ScaleFactor + sidebarHeightPadding));

        // Set buttons area
        _buttonsArea = new Rectangle(
            SidebarArea.X + scalingManager.Padding,
            SidebarArea.Y + scalingManager.Padding,
            (int)(ButtonsWidth * scalingManager.ScaleFactor),
            (int)(ButtonsHeight * scalingManager.ScaleFactor));

        // Set minimap area
        _miniMapArea = new Rectangle(
            SidebarArea.X + scalingManager.Padding,
            SidebarArea.Y + _buttonsArea.height + scalingManager.Padding * 2,
            (int)(MiniMapWidth * scalingManager.ScaleFactor),
            (int)(MiniMapHeight * scalingManager.ScaleFactor));

        // Set button positions
        var rowCounter = 0;
        var columnCounter = 0;
        foreach (var button in _buttonsList)
        {
            button.Area = new Rectangle(
                _buttonsArea.X + rowCounter * ButtonWidth * scalingManager.ScaleFactor,
                _buttonsArea.Y + columnCounter * ButtonHeight * scalingManager.ScaleFactor,
                ButtonWidth * scalingManager.ScaleFactor,
                ButtonHeight * scalingManager.ScaleFactor);

            rowCounter++;

            if (rowCounter >= ButtonsRows)
            {
                rowCounter = 0;
                columnCounter++;
            }
        }

        // Apply minimap area to texture
        _miniMapTexture.DestinationRectangle = _miniMapArea;
    }

    /// <summary>
    ///     Draws a popup message on the screen
    /// </summary>
    /// <param name="message"> String containing the popup contents </param>
    /// <param name="popupArea"> Rectangle that contains the area on screen the popup should occupy </param>
    /// <param name="scalePopup"> If true popup area will be scaled to fit contents of message </param>
    private void DrawPopUp(string message, Rectangle popupArea, bool scalePopup = false)
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var fontSize = scalingManager.FontSize;
        var fontSpacing = scalingManager.FontSpacing;
        var padding = scalingManager.Padding;

        if (scalePopup)
        {
            var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), message, fontSize, fontSpacing);
            popupArea = popupArea with
            {
                width = messageSize.X_int() + padding * 8,
                height = messageSize.Y_int() + padding * 8
            };
        }

        var innerPopupArea = new Rectangle
        (
            popupArea.X + padding,
            popupArea.Y + padding,
            popupArea.width - padding * 2,
            popupArea.height - padding * 2
        );

        Raylib.DrawRectangleRec(popupArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(innerPopupArea, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), message,
            new Vector2(popupArea.X + padding * 4, popupArea.Y + padding * 4),
            fontSize, fontSpacing, Raylib.WHITE);
    }

    /// <summary>
    ///     Unloads the map and returns to the main menu
    /// </summary>
    private void ReturnToMainMenu()
    {
        ServiceManager.GetService<StateManager>().GameState = GameState.MainMenu;
    }
}