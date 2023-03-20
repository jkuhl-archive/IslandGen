using IslandGen.Objects.UI;

namespace IslandGen.Services;

public class OverlayUi
{
    private ToolTip? _toolTip;

    public void Draw()
    {
        if (_toolTip != null)
        {
            _toolTip.Draw();
            _toolTip = null;
        }
    }

    /// <summary>
    ///     Sets a tooltip to be drawn as an overlay
    /// </summary>
    /// <param name="toolTip"> Tooltip object </param>
    public void SetToolTip(ToolTip toolTip)
    {
        _toolTip = toolTip;
    }
}