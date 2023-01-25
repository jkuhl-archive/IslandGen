using System.Numerics;
using IslandGen.Data;
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
    private bool _showDebugInfo;
    private Rectangle _sidebarArea;

    /// <summary>
    ///     Service that manages the game's UI
    /// </summary>
    public GameUi()
    {
        _buttonsList = new List<Button>
        {
            new("Zoom In", ServiceManager.GetService<GameCamera>().ZoomIn),
            new("Zoom Out", ServiceManager.GetService<GameCamera>().ZoomOut),
            new("New Island", () => ServiceManager.ReplaceService(new GameMap())),
            new("Debug Stats", () => _showDebugInfo = !_showDebugInfo),
            new("Fullscreen", Raylib.ToggleFullscreen),
            new("Exit Game", Raylib.CloseWindow)
        };
        _debugInfo = string.Empty;
        _miniMapTexture = new RenderTexturePro(new Vector2(MiniMapWidth, MiniMapHeight));
    }

    public void Draw()
    {
        // Render minimap to texture
        var gameCamera = ServiceManager.GetService<GameCamera>();
        var gameMap = ServiceManager.GetService<GameMap>();
        Raylib.BeginTextureMode(_miniMapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        for (var mapX = 0; mapX < gameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < gameMap.MapSize; mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), gameMap.TileMap[mapX, mapY].GetTileColor());
        Raylib.DrawRectangleLinesEx(gameCamera.GetCameraMapArea(), 1, Raylib.RED);
        Raylib.EndTextureMode();

        // Draw sidebar backdrop
        Raylib.DrawRectangleRec(_sidebarArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(_buttonsArea, Raylib.GRAY);

        // Draw buttons
        foreach (var button in _buttonsList) button.Draw();

        // Draw minimap
        _miniMapTexture.Draw();

        // Draw debug info
        if (_showDebugInfo) DrawPopUp(_debugInfo);
    }

    public void Update()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var sidebarWidthPadding = scalingManager.WidthPadding * SidebarWidthPaddingSegments;
        var sidebarHeightPadding = scalingManager.HeightPadding * SidebarHeightPaddingSegments;

        // Set sidebar area
        _sidebarArea = new Rectangle(
            scalingManager.WindowWidth - SidebarWidth * scalingManager.WidthScale - sidebarWidthPadding,
            scalingManager.WindowHeight - SidebarHeight * scalingManager.HeightScale - sidebarHeightPadding,
            (int)(SidebarWidth * scalingManager.WidthScale + sidebarWidthPadding),
            (int)(SidebarHeight * scalingManager.HeightScale + sidebarHeightPadding));

        // Set buttons area
        _buttonsArea = new Rectangle(
            _sidebarArea.X + scalingManager.WidthPadding,
            _sidebarArea.Y + scalingManager.HeightPadding,
            (int)(ButtonsWidth * scalingManager.WidthScale),
            (int)(ButtonsHeight * scalingManager.HeightScale));

        // Set minimap area
        _miniMapArea = new Rectangle(
            _sidebarArea.X + scalingManager.WidthPadding,
            _sidebarArea.Y + _buttonsArea.height + scalingManager.HeightPadding * 2,
            (int)(MiniMapWidth * scalingManager.WidthScale),
            (int)(MiniMapHeight * scalingManager.HeightScale));

        // Set button positions
        var rowCounter = 0;
        var columnCounter = 0;
        foreach (var button in _buttonsList)
        {
            button.Area = new Rectangle(
                _buttonsArea.X + rowCounter * ButtonWidth * scalingManager.WidthScale,
                _buttonsArea.Y + columnCounter * ButtonHeight * scalingManager.HeightScale,
                ButtonWidth * scalingManager.WidthScale,
                ButtonHeight * scalingManager.HeightScale);

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
        if (_showDebugInfo)
        {
            var gameCamera = ServiceManager.GetService<GameCamera>();
            _debugInfo =
                $"FPS: {Raylib.GetFPS()}\n" +
                $"Current Resolution: {scalingManager.WindowWidth}x{scalingManager.WindowHeight}\n" +
                $"Scaling Factor: W: {scalingManager.WidthScale}, H: {scalingManager.HeightScale}\n" +
                $"Camera Zoom: {gameCamera.Camera.zoom}\n" +
                $"Camera Target: {gameCamera.Camera.target}\n" +
                $"Camera Visible Map Tiles: {gameCamera.GetCameraMapArea().String()}";
        }
    }

    /// <summary>
    ///     Draws a popup message in the top right corner of the screen
    /// </summary>
    /// <param name="message"> String containing the popup contents </param>
    private void DrawPopUp(string message)
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var fontSize = scalingManager.FontSize;
        var widthPadding = scalingManager.WidthPadding;
        var heightPadding = scalingManager.HeightPadding;
        var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), message, fontSize, 2);

        Raylib.DrawRectangle(0, 0, messageSize.X_int() + widthPadding * 8, messageSize.Y_int() + heightPadding * 8,
            Raylib.WHITE);
        Raylib.DrawRectangle(widthPadding, heightPadding, messageSize.X_int() + widthPadding * 6,
            messageSize.Y_int() + heightPadding * 6, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), message, new Vector2(widthPadding * 4, heightPadding * 4), fontSize,
            2, Raylib.WHITE);
    }
}