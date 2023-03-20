using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class TextField
{
    private const int CursorBlinkDelay = 30;
    private static int _fontSize;
    private static int _fontSpacing;
    private static int _padding;
    private static Vector2 _textPosition;
    private readonly int _characterLimit;

    private readonly ToolTip? _toolTip;
    private int _cursorBlinkCounter;
    private bool _selected;
    private bool _showCursor;

    /// <summary>
    ///     Text field where the user can enter text with a keyboard
    /// </summary>
    /// <param name="area"> Rectangle that represents the area the text field should occupy on screen </param>
    /// <param name="toolTip"> List of strings that comprise the text field's tooltip </param>
    /// <param name="characterLimit"> Maximum number of characters that can be entered </param>
    /// <param name="contents"> Text within the text field, this allows a value to be pre-populated </param>
    public TextField(Rectangle area = new(), List<string>? toolTip = null, int characterLimit = 10,
        string contents = "")
    {
        Area = area;
        if (toolTip != null) _toolTip = new ToolTip(toolTip);
        _characterLimit = characterLimit;
        Contents = contents;
    }

    public string Contents { get; set; }
    public Rectangle Area { get; private set; }
    public bool MouseOver { get; private set; }

    public void Draw()
    {
        var boxColor = Colors.TextFieldBorder;
        if (MouseOver) boxColor = Colors.TextFieldBorderSelected;

        var contents = Contents;
        if (_selected && _showCursor) contents += "_";

        _textPosition = new Vector2(Area.X + _padding * 4, Area.Center().Y - _fontSize / 2);

        Raylib.DrawRectangleRec(Area, Colors.TextFieldBackdrop);
        Raylib.DrawRectangleLinesEx(Area, _padding, boxColor);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), contents, _textPosition, _fontSize, _fontSpacing, Raylib.BLACK);
    }

    public void Update()
    {
        // Check if mouse is over text field, it not reset tooltip counter and return
        MouseOver = Area.PointInsideRectangle(InputManager.GetMousePosition());
        if (MouseOver)
            _toolTip?.IncrementCounter();
        else
            _toolTip?.ResetCounter();

        // If mouse button clicked select the text field
        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) _selected = MouseOver;

        // If not selected return now
        if (!_selected) return;

        // Handle cursor blink
        _cursorBlinkCounter++;
        if (_cursorBlinkCounter >= CursorBlinkDelay)
        {
            _showCursor = !_showCursor;
            _cursorBlinkCounter = 0;
        }

        // Process character input
        foreach (var pressedChar in InputManager.PressedChars)
        {
            if (Contents.Length >= _characterLimit) break;
            Contents += pressedChar;
        }

        // Process backspace input
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            if (Contents.Length > 0)
                Contents = Contents[..^1];
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public static void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _fontSize = scalingManager.FontSize * 2;
        _fontSpacing = scalingManager.FontSpacing;
        _padding = scalingManager.Padding;
    }

    /// <summary>
    ///     Sets the area the text field should occupy on screen
    /// </summary>
    /// <param name="area"> Rectangle representing the text field's desired area </param>
    public void SetArea(Rectangle area)
    {
        Area = area;
    }
}