using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameUi
{
    private const string ControlsMessage = "Left Mouse to generate a new map\n" +
                                           "F1 to toggle fullscreen\n" +
                                           "PageUp / PageDown to zoom\n" +
                                           "Arrow Keys move map";

    private readonly RenderTexturePro _miniMapTexture;

    /// <summary>
    ///     Constructor for GameUi
    /// </summary>
    public GameUi()
    {
        _miniMapTexture = new RenderTexturePro(new Vector2(ServiceManager.GetService<GameMap>().MapSize));
    }

    public void Draw()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();

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

        // Draw minimap backdrop
        var miniMapPosition = new Vector2(
            scalingManager.WindowWidth - _miniMapTexture.RenderTexture.texture.width * scalingManager.WidthScale -
            scalingManager.WidthPadding * 2,
            scalingManager.WindowHeight - _miniMapTexture.RenderTexture.texture.height * scalingManager.HeightScale -
            scalingManager.HeightPadding * 2);
        Raylib.DrawRectangle(miniMapPosition.X_int(), miniMapPosition.Y_int(),
            (int)(_miniMapTexture.RenderTexture.texture.width * scalingManager.WidthScale +
                  scalingManager.WidthPadding * 2),
            (int)(_miniMapTexture.RenderTexture.texture.height * scalingManager.WidthScale +
                  scalingManager.HeightPadding * 2),
            Raylib.WHITE);

        // Draw minimap
        _miniMapTexture.DestinationRectangle.X = miniMapPosition.X + scalingManager.WidthPadding;
        _miniMapTexture.DestinationRectangle.Y = miniMapPosition.Y + scalingManager.HeightPadding;
        _miniMapTexture.Draw();

        // Print status popup
        var statusMessage =
            $"FPS: {Raylib.GetFPS()}\n" +
            $"Scaling Factor: W: {scalingManager.WidthScale}, H: {scalingManager.HeightScale}\n" +
            $"Camera Zoom: {gameCamera.Camera.zoom}\n" +
            $"Camera Target: {gameCamera.Camera.target}\n" +
            $"Camera Visible Map Tiles: {gameCamera.GetCameraMapArea().String()}\n\n" +
            ControlsMessage;
        DrawPopUp(statusMessage, scalingManager.FontSize, scalingManager.WidthPadding,
            scalingManager.HeightPadding);
    }

    /// <summary>
    ///     Draws a popup message in the top right corner of the screen
    /// </summary>
    /// <param name="message"> String containing the popup contents </param>
    /// <param name="fontSize"> Font size the popup should use </param>
    /// <param name="widthPadding"> Amount of padding on the left and right sides of the popup </param>
    /// <param name="heightPadding"> Amount of padding on the top and right bottom of the popup </param>
    private void DrawPopUp(string message, int fontSize, int widthPadding, int heightPadding)
    {
        var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), message, fontSize, 2);
        Raylib.DrawRectangle(0, 0, messageSize.X_int() + widthPadding * 8, messageSize.Y_int() + heightPadding * 8,
            Raylib.WHITE);
        Raylib.DrawRectangle(widthPadding, heightPadding, messageSize.X_int() + widthPadding * 6,
            messageSize.Y_int() + heightPadding * 6, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), message, new Vector2(widthPadding * 4, heightPadding * 4), fontSize,
            2, Raylib.WHITE);
    }
}