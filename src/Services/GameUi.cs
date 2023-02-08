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
    private const int SidebarWidth = 100;
    private const int SidebarHeight = 200;
    private const int SidebarWidthPaddingSegments = 2;
    private const int SidebarHeightPaddingSegments = 3;
    private const int MiniMapWidth = 100;
    private const int MiniMapHeight = 100;
    private const int ButtonsWidth = 100;
    private const int ButtonsHeight = 100;
    private const int ButtonWidth = 50;
    private const int ButtonHeight = 20;
    private const int ButtonsRows = ButtonsWidth / ButtonWidth;
    private const int ButtonsColumns = ButtonsHeight / ButtonHeight;

    private readonly List<Button> _buttonsList;
    private readonly RenderTexturePro _miniMapTexture;

    private Rectangle _buttonsArea;
    private string _debugInfo;
    private Rectangle _miniMapArea;

    /// <summary>
    ///     Service that manages the game's UI
    /// </summary>
    public GameUi()
    {
        _buttonsList = new List<Button>
        {
            new("Zoom In", ServiceManager.GetService<GameCamera>().ZoomIn),
            new("Zoom Out", ServiceManager.GetService<GameCamera>().ZoomOut),
            new("Save Island", StateManager.SaveGame),
            new("Load Island", StateManager.LoadGame),
            new("New Island", StateManager.NewGame),
            new("Change Speed", ChangeSpeed),
            new("Debug Mode",
                () => ServiceManager.GetService<GameSettings>().DebugMode =
                    !ServiceManager.GetService<GameSettings>().DebugMode),
            new("Fullscreen", Raylib.ToggleFullscreen),
            new("New Shelter", () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Shelter())),
            new("Main Menu", ReturnToMainMenu)
        };
        _debugInfo = string.Empty;
        _miniMapTexture = new RenderTexturePro(new Vector2(MiniMapWidth, MiniMapHeight));
    }

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

        // Draw sidebar backdrop
        Raylib.DrawRectangleRec(SidebarArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(_buttonsArea, Raylib.GRAY);

        // Draw buttons
        foreach (var button in _buttonsList) button.Draw();

        // Draw minimap
        _miniMapTexture.Draw();

        // Draw debug info
        if (gameSettings.DebugMode) DrawPopUp(_debugInfo);
    }

    public void Update()
    {
        var gameSettings = ServiceManager.GetService<GameSettings>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var sidebarWidthPadding = scalingManager.Padding * SidebarWidthPaddingSegments;
        var sidebarHeightPadding = scalingManager.Padding * SidebarHeightPaddingSegments;

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

        // Set debug info
        if (gameSettings.DebugMode)
        {
            var gameCamera = ServiceManager.GetService<GameCamera>();
            var gameLogic = ServiceManager.GetService<GameLogic>();
            var gameMap = ServiceManager.GetService<GameMap>();

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
    ///     Toggles current GameSpeed to the next value
    /// </summary>
    private void ChangeSpeed()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        gameLogic.GameSpeed = gameLogic.GameSpeed.GetNext();
    }

    /// <summary>
    ///     Draws a popup message in the top right corner of the screen
    /// </summary>
    /// <param name="message"> String containing the popup contents </param>
    private void DrawPopUp(string message)
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var fontSize = scalingManager.FontSize;
        var fontSpacing = scalingManager.FontSpacing;
        var padding = scalingManager.Padding;
        var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), message, fontSize, fontSpacing);

        Raylib.DrawRectangle(0, 0, messageSize.X_int() + padding * 8, messageSize.Y_int() + padding * 8,
            Raylib.WHITE);
        Raylib.DrawRectangle(padding, padding, messageSize.X_int() + padding * 6,
            messageSize.Y_int() + padding * 6, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), message, new Vector2(padding * 4, padding * 4), fontSize,
            fontSpacing, Raylib.WHITE);
    }

    /// <summary>
    ///     Unloads the map and returns to the main menu
    /// </summary>
    private void ReturnToMainMenu()
    {
        ServiceManager.GetService<StateManager>().GameState = GameState.MainMenu;
    }
}