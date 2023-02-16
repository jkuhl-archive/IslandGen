using IslandGen.Data;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public class Button
{
    private readonly Action _function;
    private readonly bool _settingsButton;

    public Rectangle Area;
    public string Label;

    /// <summary>
    ///     Wrapper for RayGui.GuiButton that triggers a function when the button is clicked
    /// </summary>
    /// <param name="label"> Text that will be displayed on the button </param>
    /// <param name="function"> Function that should be executed when the button is clicked </param>
    /// <param name="area"> Rectangle that represents the area the button should occupy on screen </param>
    /// <param name="settingsButton"> Denotes if this button should be active when the settings menu is active </param>
    public Button(string label, Action function, Rectangle area = new(), bool settingsButton = false)
    {
        Label = label;
        _function = function;
        _settingsButton = settingsButton;
        Area = area;
    }

    public void Draw()
    {
        if (ServiceManager.GetService<GameSettingsUi>().SettingsMenuActive != _settingsButton) return;
        if (RayGui.GuiButton(Area, Label))
        {
            _function();
            Raylib.PlaySound(Assets.Sounds["click"]);
        }
    }
}