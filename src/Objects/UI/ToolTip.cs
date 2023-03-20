using System.Numerics;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class ToolTip
{
    private const int Delay = 30;
    private static int _fontSize;
    private static int _fontSpacing;
    private static int _padding;
    private static int _windowHeight;
    private static int _windowWidth;

    private List<string> _contents;
    private int _counter;

    /// <summary>
    ///     UI element that shows informational text about another UI element
    /// </summary>
    /// <param name="contents"> List of strings that comprise the tooltip </param>
    public ToolTip(List<string> contents)
    {
        _contents = contents;
    }

    public void Draw()
    {
        var contents = string.Join("\n", _contents);
        var size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), contents, _fontSize, _fontSpacing);
        var mousePosition = InputManager.GetMousePosition();

        // Set tooltip area
        var area = new Rectangle(
            mousePosition.X,
            mousePosition.Y,
            size.X + _padding * 8,
            size.Y + _padding * 8);

        // Adjust drawing position to prevent drawing off screen
        if (area.X + area.width > _windowWidth) area.X -= area.width;
        if (area.Y + area.height > _windowHeight) area.Y -= area.height;

        // Set tooltip inner area
        var innerArea = new Rectangle(
            area.X + _padding,
            area.Y + _padding,
            area.width - _padding * 2,
            area.height - _padding * 2);

        // Draw tooltip
        Raylib.DrawRectangleRec(area, Raylib.WHITE);
        Raylib.DrawRectangleRec(innerArea, Raylib.BLACK);
        Raylib.DrawTextEx(
            Raylib.GetFontDefault(),
            contents,
            new Vector2(area.X + _padding * 4, area.Y + _padding * 4),
            _fontSize,
            _fontSpacing,
            Raylib.WHITE);
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public static void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _fontSize = scalingManager.FontSize;
        _fontSpacing = scalingManager.FontSpacing;
        _padding = scalingManager.Padding;
        _windowHeight = scalingManager.WindowHeight;
        _windowWidth = scalingManager.WindowWidth;
    }

    /// <summary>
    ///     Increments the tooltip's counter, if counter is above delay threshold then set the tooltip to be drawn
    /// </summary>
    public void IncrementCounter()
    {
        _counter++;
        if (_counter >= Delay) ServiceManager.GetService<OverlayUi>().SetToolTip(this);
    }

    /// <summary>
    ///     Resets the tooltip's counter
    /// </summary>
    public void ResetCounter()
    {
        _counter = 0;
    }

    /// <summary>
    ///     Sets the tooltip's contents
    /// </summary>
    /// <param name="contents"> List of strings that comprise the tooltip </param>
    public void SetContents(List<string> contents)
    {
        _contents = contents;
    }
}