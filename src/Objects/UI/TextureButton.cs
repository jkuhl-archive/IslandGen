using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class TextureButton
{
    private readonly Action _function;
    private readonly bool _settingsButton;
    private readonly ToolTip? _toolTip;
    private Rectangle _textureSource;

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
        if (toolTip != null) _toolTip = new ToolTip(toolTip);
        Disabled = disabled;
        _settingsButton = settingsButton;

        _textureSource = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    public Rectangle Area { get; private set; }
    public bool Disabled { get; set; }
    public bool MouseOver { get; private set; }
    public bool MouseDown { get; private set; }
    public Texture Texture { get; private set; }

    public void Draw()
    {
        // Set button tint based on current state
        var tint = Raylib.WHITE;
        if (MouseDown) tint = Colors.ButtonTintMouseDown;
        else if (MouseOver) tint = Colors.ButtonTintMouseOver;
        if (Disabled) tint = Colors.ButtonTintMouseDown;

        // Draw button texture
        Raylib.DrawTexturePro(Texture, _textureSource, Area, Vector2.Zero, 0.0f, tint);
    }

    public void Update()
    {
        MouseOver = false;
        MouseDown = false;

        // If settings menu is open and this button is not part of the settings menu, return without doing anything
        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive != _settingsButton) return;

        // Check if mouse is over button, it not reset tooltip counter and return
        MouseOver = Area.PointInsideRectangle(InputManager.GetMousePosition());
        if (!MouseOver)
        {
            _toolTip?.ResetCounter();
            return;
        }

        // If mouse is over button increment tool tip counter
        _toolTip?.IncrementCounter();

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
}