using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameUi
{
    private const double BaseWidth = 640;
    private const double BaseHeight4by3 = 480;
    private const double BaseHeight16by9 = 360;
    private const double BaseHeight16by10 = 400;

    private const double AspectRatio4by3 = BaseWidth / BaseHeight4by3;
    private const double AspectRatio16by9 = BaseWidth / BaseHeight16by9;
    private const double AspectRatio16by10 = BaseWidth / BaseHeight16by10;
    private const double WidthScaleFactor = 1 / BaseWidth;
    private const double HeightScaleFactor4by3 = 1 / BaseHeight4by3;
    private const double HeightScaleFactor16b9 = 1 / BaseHeight16by9;
    private const double HeightScaleFactor16b10 = 1 / BaseHeight16by10;

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

    /// <summary>
    ///     Calculates a vertical scaling factor for the current game resolution
    /// </summary>
    /// <returns> Float that represents the vertical scaling factor </returns>
    private float GetHeightScale()
    {
        return ((double)Raylib.GetRenderWidth() / Raylib.GetRenderHeight()) switch
        {
            AspectRatio4by3 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor4by3, 2),
            AspectRatio16by9 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor16b9, 2),
            AspectRatio16by10 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor16b10, 2),
            _ => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor4by3, 2)
        };
    }

    /// <summary>
    ///     Calculates a horizontal scaling factor for the current game resolution
    /// </summary>
    /// <returns> Float that represents the horizontal scaling factor </returns>
    private float GetWidthScale()
    {
        return (float)Math.Round(Raylib.GetRenderWidth() * WidthScaleFactor, 2);
    }

    public void Draw()
    {
        // Calculate current window size, scale, and padding
        var windowWidth = Raylib.GetRenderWidth();
        var windowHeight = Raylib.GetRenderHeight();
        var widthScale = GetWidthScale();
        var heightScale = GetHeightScale();
        var widthPadding = (int)(1 * widthScale);
        var heightPadding = (int)(1 * heightScale);
        var fontSize = (int)((widthScale + heightScale) / 2 * 10);

        // Render minimap to texture
        var gameMap = ServiceManager.GetService<GameMap>();
        Raylib.BeginTextureMode(_miniMapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        for (var mapX = 0; mapX < gameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < gameMap.MapSize; mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), gameMap.TileMap[mapX, mapY].GetTileColor());
        Raylib.EndTextureMode();

        // Draw minimap backdrop
        var miniMapPosition = new Vector2(
            windowWidth - _miniMapTexture.RenderTexture.texture.width * widthScale - widthPadding * 2,
            windowHeight - _miniMapTexture.RenderTexture.texture.height * heightScale - heightPadding * 2);
        Raylib.DrawRectangle(miniMapPosition.X_int(), miniMapPosition.Y_int(),
            (int)(_miniMapTexture.RenderTexture.texture.width * widthScale + widthPadding * 2),
            (int)(_miniMapTexture.RenderTexture.texture.height * heightScale + heightPadding * 2),
            Raylib.WHITE);

        // Draw minimap
        _miniMapTexture.WidthScale = widthScale;
        _miniMapTexture.HeightScale = heightScale;
        _miniMapTexture.DestinationRectangle.X = miniMapPosition.X + widthPadding;
        _miniMapTexture.DestinationRectangle.Y = miniMapPosition.Y + heightPadding;
        _miniMapTexture.Draw();

        DrawPopUp($"FPS: {Raylib.GetFPS()}\n" + ControlsMessage, fontSize, widthPadding, heightPadding);
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