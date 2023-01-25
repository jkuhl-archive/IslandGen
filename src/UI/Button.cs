using Raylib_CsLo;

namespace IslandGen.UI;

public class Button
{
    private readonly Action _function;
    private readonly string _label;

    public Rectangle Area;

    /// <summary>
    ///     Wrapper for RayGui.GuiButton that triggers a function when the button is clicked
    /// </summary>
    /// <param name="label"> Text that will be displayed on the button </param>
    /// <param name="function"> Function that should be executed when the button is clicked </param>
    /// <param name="area"> Rectangle that represents the area the button should occupy on screen </param>
    public Button(string label, Action function, Rectangle area = new())
    {
        _label = label;
        _function = function;
        Area = area;
    }

    public void Draw()
    {
        if (RayGui.GuiButton(Area, _label)) _function();
    }
}