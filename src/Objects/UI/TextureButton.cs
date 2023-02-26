using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class TextureButton
{
    private const int ToolTipDelay = 30;
    private const int ToolTipMouseOffset = 8;
    private static int _toolTipFontSize;
    private static int _toolTipFontSpacing;
    private static int _toolTipPadding;
    private static int _windowWidth;

    private readonly Action _function;
    private readonly bool _settingsButton;
    private bool _displayToolTip;
    private Rectangle _textureSource;
    private int _toolTipCounter;

    /// <summary>
    ///     Simple button with a texture
    /// </summary>
    /// <param name="texture"> Texture the button should use </param>
    /// <param name="function"> Function that should be executed when the button is clicked </param>
    /// <param name="area"> Rectangle that represents the area the button should occupy on screen </param>
    /// <param name="toolTip"> List of strings that comprise the button's tooltip </param>
    /// <param name="disabled"> Denotes is this button should initialize as disabled </param>
    /// <param name="settingsButton"> Denotes if this button should be active when the settings menu is active </param>
    public TextureButton(Texture texture, Action function, Rectangle area = new(), List<string>? toolTip = null,
        bool disabled = false, bool settingsButton = false)
    {
        Texture = texture;
        _function = function;
        Area = area;
        ToolTip = toolTip;
        Disabled = disabled;
        _settingsButton = settingsButton;

        _textureSource = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    public Rectangle Area { get; private set; }
    public bool Disabled { get; set; }
    public bool MouseOver { get; private set; }
    public bool MouseDown { get; private set; }
    public Texture Texture { get; private set; }
    public List<string>? ToolTip { get; private set; }

    public void Draw()
    {
        var tint = Raylib.WHITE;
        if (MouseDown) tint = Colors.ButtonTintMouseDown;
        else if (MouseOver) tint = Colors.ButtonTintMouseOver;
        if (Disabled) tint = Colors.ButtonTintMouseDown;

        Raylib.DrawTexturePro(Texture, _textureSource, Area, Vector2.Zero, 0.0f, tint);
        if (ToolTip != null && _displayToolTip)
        {
            var contents = string.Join("\n", ToolTip);
            var size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), contents, _toolTipFontSize, _toolTipFontSpacing);
            var mousePosition = InputManager.GetMousePosition();

            // Set tooltip area, flip to drawing on left of mouse if tooltip will be drawn outside the game window
            var toolTipArea = new Rectangle(
                mousePosition.X,
                mousePosition.Y,
                size.X + _toolTipPadding * 8,
                size.Y + _toolTipPadding * 8);
            if (toolTipArea.X + toolTipArea.width > _windowWidth) toolTipArea.X -= toolTipArea.width;

            // Set tooltip inner area
            var innerToolTipArea = new Rectangle
            (
                toolTipArea.X + _toolTipPadding,
                toolTipArea.Y + _toolTipPadding,
                toolTipArea.width - _toolTipPadding * 2,
                toolTipArea.height - _toolTipPadding * 2);

            // Draw tooltip
            Raylib.DrawRectangleRec(toolTipArea, Raylib.WHITE);
            Raylib.DrawRectangleRec(innerToolTipArea, Raylib.BLACK);
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                contents,
                new Vector2(toolTipArea.X + _toolTipPadding * 4, toolTipArea.Y + _toolTipPadding * 4),
                _toolTipFontSize,
                _toolTipFontSpacing,
                Raylib.WHITE);
        }
    }

    public void Update()
    {
        _displayToolTip = false;
        MouseOver = false;
        MouseDown = false;

        // If settings menu is open and this button is not part of the settings menu, return without doing anything
        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive != _settingsButton) return;

        // Check if mouse is over button, it not reset tooltip counter and return
        MouseOver = Area.PointInsideRectangle(InputManager.GetMousePosition());
        if (!MouseOver)
        {
            _toolTipCounter = 0;
            return;
        }

        // Increment tooltip counter, display tooltip if mouse has been over button long enough
        _toolTipCounter++;
        if (_toolTipCounter >= ToolTipDelay) _displayToolTip = true;

        // If button is disabled return
        if (Disabled) return;

        // Check if mouse button is being held down
        MouseDown = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);

        // If mouse button clicked trigger the button's function
        if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            _function();
            Raylib.PlaySound(Assets.Sounds["click"]);
        }
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public static void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _toolTipFontSize = scalingManager.FontSize;
        _toolTipFontSpacing = scalingManager.FontSpacing;
        _toolTipPadding = scalingManager.Padding;
        _windowWidth = scalingManager.WindowWidth;
    }

    /// <summary>
    ///     Sets the area the button should occupy on screen
    /// </summary>
    /// <param name="area"> Rectangle representing the button's desired area </param>
    public void SetArea(Rectangle area)
    {
        Area = area;
    }

    /// <summary>
    ///     Sets the button's texture
    /// </summary>
    /// <param name="texture"> Texture the button should use </param>
    public void SetTexture(Texture texture)
    {
        Texture = texture;
        _textureSource = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    /// <summary>
    ///     Sets the button's tooltip
    /// </summary>
    /// <param name="toolTip"> List of strings that comprise the button's tooltip </param>
    public void SetToolTip(List<string> toolTip)
    {
        ToolTip = toolTip;
    }
}