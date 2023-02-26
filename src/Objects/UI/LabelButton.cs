using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class LabelButton
{
    private static int _borderWidth;
    private readonly Action _function;
    private readonly bool _settingsButton;
    private int _labelFontSize;
    private Vector2 _labelPosition;

    /// <summary>
    ///     Simple button with a text label, these are mostly placeholders until textures are available
    /// </summary>
    /// <param name="label"> Text that will be displayed on the button </param>
    /// <param name="function"> Function that should be executed when the button is clicked </param>
    /// <param name="area"> Rectangle that represents the area the button should occupy on screen </param>
    /// <param name="settingsButton"> Denotes if this button should be active when the settings menu is active </param>
    public LabelButton(string label, Action function, Rectangle area = new(), bool settingsButton = false)
    {
        Label = label;
        _function = function;
        _settingsButton = settingsButton;
        Area = area;

        UpdateLabelSize();
    }

    public Rectangle Area { get; private set; }
    public bool MouseOver { get; private set; }
    public bool MouseDown { get; private set; }
    public string Label { get; private set; }

    public void Draw()
    {
        Raylib.DrawRectangleRec(Area, Colors.ButtonDefaultColor);
        Raylib.DrawRectangleLinesEx(Area, _borderWidth, Raylib.WHITE);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), Label, _labelPosition, _labelFontSize, 2, Raylib.BLACK);
        if (MouseDown) Raylib.DrawRectangleRec(Area, Colors.ButtonHighlightMouseDown);
        else if (MouseOver) Raylib.DrawRectangleRec(Area, Colors.ButtonHighlightMouseOver);
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
    ///     Recalculates scaled UI elements
    /// </summary>
    public static void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _borderWidth = scalingManager.Padding;
    }

    /// <summary>
    ///     Sets the area the button should occupy on screen
    /// </summary>
    /// <param name="area"> Rectangle representing the button's desired area </param>
    public void SetArea(Rectangle area)
    {
        Area = area;
        UpdateLabelSize();
    }

    /// <summary>
    ///     Sets the label displayed on the button
    /// </summary>
    /// <param name="label"> String containing the button's label text </param>
    public void SetLabel(string label)
    {
        Label = label;
        UpdateLabelSize();
    }

    /// <summary>
    ///     Calculates the proper size for the label text to fit within the button's area
    /// </summary>
    private void UpdateLabelSize()
    {
        _labelFontSize = (int)(Area.width + Area.height) / 7;
        _labelFontSize -= _labelFontSize % 5;
        var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Label, _labelFontSize, 2);

        while (messageSize.X > Area.width - Area.width / 10 || messageSize.Y > Area.height - Area.height / 10)
        {
            _labelFontSize -= 5;
            messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Label, _labelFontSize, 2);
        }

        _labelPosition = Area.Center() - messageSize / 2;
    }
}