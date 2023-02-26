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
    private Rectangle _textureSource;

    /// <summary>
    ///     Simple button with a texture
    /// </summary>
    /// <param name="texture"> Texture the button should use </param>
    /// <param name="function"> Function that should be executed when the button is clicked </param>
    /// <param name="area"> Rectangle that represents the area the button should occupy on screen </param>
    /// <param name="settingsButton"> Denotes if this button should be active when the settings menu is active </param>
    public TextureButton(Texture texture, Action function, Rectangle area = new(), bool settingsButton = false)
    {
        Texture = texture;
        _function = function;
        _settingsButton = settingsButton;
        Area = area;

        _textureSource = new Rectangle(0, 0, Texture.width, Texture.height);
    }

    public Rectangle Area { get; private set; }
    public bool MouseOver { get; private set; }
    public bool MouseDown { get; private set; }
    public Texture Texture { get; private set; }

    public void Draw()
    {
        var tint = Raylib.WHITE;
        if (MouseDown) tint = Colors.ButtonTintMouseDown;
        else if (MouseOver) tint = Colors.ButtonTintMouseOver;

        Raylib.DrawTexturePro(Texture, _textureSource, Area, Vector2.Zero, 0.0f, tint);
    }

    public void Update()
    {
        MouseOver = false;
        MouseDown = false;

        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive != _settingsButton) return;

        MouseOver = Area.PointInsideRectangle(InputManager.GetMousePosition());
        if (!MouseOver) return;

        MouseDown = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);

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