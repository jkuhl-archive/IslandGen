using System.Numerics;
using IslandGen.Data.ECS.Entities.Creatures;
using IslandGen.Data.ECS.Entities.Structures;
using IslandGen.Data.Enum;
using IslandGen.Data.Textures;
using IslandGen.Extensions;
using IslandGen.UI;
using IslandGen.Utils;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameUi
{
    private const int CalendarWidth = 115;
    private const int CalendarHeight = 20;
    private const int MiniMapWidth = 100;
    private const int MiniMapHeight = 100;
    private const int SelectedEntityMenuWidth = 250;
    private const int SelectedEntityMenuHeight = 80;
    private const int SidebarButtonsWidth = 100;
    private const int SidebarButtonsHeight = 100;
    private const int SidebarButtonWidth = 50;
    private const int SidebarButtonHeight = 20;
    private const int SidebarButtonsRows = SidebarButtonsWidth / SidebarButtonWidth;
    private const int SidebarButtonsColumns = SidebarButtonsHeight / SidebarButtonHeight;
    private const int SidebarWidth = 100;
    private const int SidebarHeight = 200;
    private const int SidebarWidthPaddingSegments = 2;
    private const int SidebarHeightPaddingSegments = 3;
    private const int SpeedControlsButtonWidth = 25;
    private const int SpeedControlsWidth = SpeedControlsButtonWidth * 3;
    private const int SpeedControlsHeight = 15;

    private const string PausedString = "Paused";
    private readonly RenderTexturePro _miniMapTexture;

    private readonly List<Button> _sidebarButtonsList;
    private readonly List<Button> _speedControlButtonsList;
    private string _calendarString;
    private string _debugInfoString;
    private Rectangle _miniMapArea;
    private string? _selectedEntityString;

    private Rectangle _sidebarButtonsArea;

    /// <summary>
    ///     Service that manages the game's UI
    /// </summary>
    public GameUi()
    {
        _sidebarButtonsList = new List<Button>
        {
            new("Save Island", SaveUtils.SaveGame),
            new("Load Island", SaveUtils.LoadGame),
            new("New Island", SaveUtils.NewGame),
            new("New Shelter", () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Shelter())),
            new("Settings", () => ServiceManager.GetService<GameSettingsUi>().ToggleSettingsMenu()),
            new("Main Menu", () => ServiceManager.GetService<StateManager>().SetGameState(GameState.MainMenu))
        };
        _speedControlButtonsList = new List<Button>
        {
            new("<|<|", () => ServiceManager.GetService<GameLogic>().DecreaseGameSpeed()),
            new("| |", () => ServiceManager.GetService<GameLogic>().ToggleGamePaused()),
            new("|>|>", () => ServiceManager.GetService<GameLogic>().IncreaseGameSpeed())
        };

        _calendarString = string.Empty;
        _debugInfoString = string.Empty;
        _miniMapTexture = new RenderTexturePro(new Vector2(MiniMapWidth, MiniMapHeight));
    }

    public Rectangle CalendarArea { get; private set; }
    public Rectangle SelectedEntityMenuArea { get; private set; }
    public Rectangle SidebarArea { get; private set; }
    public Rectangle SpeedControlsArea { get; private set; }

    public void Draw()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameMap = ServiceManager.GetService<GameMap>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Draw calendar
        DrawPopUp(_calendarString, CalendarArea);

        // Draw speed controls
        foreach (var button in _speedControlButtonsList) button.Draw();

        // Draw selected entity menu
        if (_selectedEntityString != null) DrawPopUp(_selectedEntityString, SelectedEntityMenuArea);

        // Start minimap texture rendering
        Raylib.BeginTextureMode(_miniMapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);

        // Render map to minimap texture
        for (var mapX = 0; mapX < gameMap.GetMapSize(); mapX++)
        for (var mapY = 0; mapY < gameMap.GetMapSize(); mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), gameMap.GetTileType((mapX, mapY)).GetTileColor());

        // Render structures to minimap texture
        foreach (var structure in gameLogic.GetEntityBaseTypeList<StructureBase>())
        foreach (var tile in structure.GetOccupiedTiles())
            Raylib.DrawPixelV(new Vector2(tile.Item1, tile.Item2), structure.MiniMapColor);

        // Render colonists to minimap texture
        foreach (var colonist in gameLogic.GetEntityList<Colonist>())
            Raylib.DrawPixelV(new Vector2(colonist.MapPosition.Item1, colonist.MapPosition.Item2),
                colonist.MiniMapColor);

        // Draw box on minimap texture that represents the area of the map currently visible
        Raylib.DrawRectangleLinesEx(gameMap.GetVisibleMapTiles(), 1, Raylib.RED);

        // End minimap texture rendering
        Raylib.EndTextureMode();

        // Draw sidebar backdrop
        Raylib.DrawRectangleRec(SidebarArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(_sidebarButtonsArea, Raylib.GRAY);

        // Draw buttons
        foreach (var button in _sidebarButtonsList) button.Draw();

        // Draw minimap
        _miniMapTexture.Draw();

        // Draw debug info
        if (gameSettings.DebugMode) DrawPopUp(_debugInfoString, new Rectangle(), true);
    }

    public void Update()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Set calendar string
        _calendarString = $"{gameLogic.CurrentDateTime.ToLongDateString()}\n" +
                          $"{gameLogic.CurrentDateTime.ToLongTimeString()}";

        // Append paused string if game is paused
        if (gameLogic.GamePaused)
            _calendarString += $" - {PausedString}";
        else
            _calendarString += $" - {gameLogic.GameSpeed}";

        // Set selected entity string
        _selectedEntityString = gameLogic.SelectedEntity?.GetInfoString();

        // Set debug info string
        if (gameSettings.DebugMode)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            var scalingManager = ServiceManager.GetService<ScalingManager>();

            _debugInfoString =
                $"FPS: {Raylib.GetFPS()}\n" +
                $"Window Resolution: {scalingManager.WindowWidth}x{scalingManager.WindowHeight}\n" +
                $"Scaling Factor: {scalingManager.ScaleFactor}\n" +
                "\n" +
                $"Mouse Window Position: {InputManager.GetMousePosition()}\n" +
                $"Mouse Map Position: {gameMap.GetMapMousePosition()}\n" +
                $"Mouse Highlighted Tile: {gameMap.GetMapMouseTile()}\n" +
                "\n" +
                $"Camera Zoom: {gameLogic.GameCamera.Camera.zoom}x\n" +
                $"Camera Target: {gameLogic.GameCamera.Camera.target}\n" +
                $"Camera Pan Limits: {gameMap.GetCameraPanLimits()}\n" +
                $"Camera Visible Map Tiles: {gameMap.GetVisibleMapTiles().String()}";
        }
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var sidebarWidthPadding = scalingManager.Padding * SidebarWidthPaddingSegments;
        var sidebarHeightPadding = scalingManager.Padding * SidebarHeightPaddingSegments;

        // Set calendar area
        CalendarArea = new Rectangle(
            scalingManager.WindowWidth - CalendarWidth * scalingManager.ScaleFactor,
            0,
            (int)(CalendarWidth * scalingManager.ScaleFactor),
            (int)(CalendarHeight * scalingManager.ScaleFactor));

        // Set selected entity menu area
        SelectedEntityMenuArea = new Rectangle(
            (scalingManager.WindowWidth - SelectedEntityMenuWidth * scalingManager.ScaleFactor) / 2,
            scalingManager.WindowHeight - SelectedEntityMenuHeight * scalingManager.ScaleFactor,
            (int)(SelectedEntityMenuWidth * scalingManager.ScaleFactor),
            (int)(SelectedEntityMenuHeight * scalingManager.ScaleFactor));

        // Set sidebar area
        SidebarArea = new Rectangle(
            scalingManager.WindowWidth - SidebarWidth * scalingManager.ScaleFactor - sidebarWidthPadding,
            scalingManager.WindowHeight - SidebarHeight * scalingManager.ScaleFactor - sidebarHeightPadding,
            (int)(SidebarWidth * scalingManager.ScaleFactor + sidebarWidthPadding),
            (int)(SidebarHeight * scalingManager.ScaleFactor + sidebarHeightPadding));

        // Set sidebar buttons area
        _sidebarButtonsArea = new Rectangle(
            SidebarArea.X + scalingManager.Padding,
            SidebarArea.Y + scalingManager.Padding,
            (int)(SidebarButtonsWidth * scalingManager.ScaleFactor),
            (int)(SidebarButtonsHeight * scalingManager.ScaleFactor));

        // Set minimap area
        _miniMapArea = new Rectangle(
            SidebarArea.X + scalingManager.Padding,
            SidebarArea.Y + _sidebarButtonsArea.height + scalingManager.Padding * 2,
            (int)(MiniMapWidth * scalingManager.ScaleFactor),
            (int)(MiniMapHeight * scalingManager.ScaleFactor));

        // Set speed controls area
        SpeedControlsArea = new Rectangle(
            SidebarArea.X + (SidebarArea.width - SpeedControlsWidth * scalingManager.ScaleFactor) / 2,
            SidebarArea.y - SpeedControlsHeight * scalingManager.ScaleFactor - scalingManager.Padding,
            (int)(SpeedControlsWidth * scalingManager.ScaleFactor),
            (int)(SpeedControlsHeight * scalingManager.ScaleFactor)
        );

        // Set sidebar button positions
        var rowCounter = 0;
        var columnCounter = 0;
        foreach (var button in _sidebarButtonsList)
        {
            button.Area = new Rectangle(
                _sidebarButtonsArea.X + rowCounter * SidebarButtonWidth * scalingManager.ScaleFactor,
                _sidebarButtonsArea.Y + columnCounter * SidebarButtonHeight * scalingManager.ScaleFactor,
                SidebarButtonWidth * scalingManager.ScaleFactor,
                SidebarButtonHeight * scalingManager.ScaleFactor);

            rowCounter++;

            if (rowCounter >= SidebarButtonsRows)
            {
                rowCounter = 0;
                columnCounter++;
            }
        }

        // Set speed controls button positions
        for (var i = 0; i < _speedControlButtonsList.Count; i++)
            _speedControlButtonsList[i].Area = new Rectangle(
                SpeedControlsArea.X + i * SpeedControlsButtonWidth * scalingManager.ScaleFactor,
                SpeedControlsArea.Y,
                SpeedControlsButtonWidth * scalingManager.ScaleFactor,
                SpeedControlsArea.height);

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
}