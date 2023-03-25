using IslandGen.Objects.UI;
using Raylib_CsLo;

namespace IslandGen.Services;

public class ScalingManager
{
    private const double BaseWidth = 640;
    private const double BaseHeight4By3 = 480;
    private const double BaseHeight16By9 = 360;
    private const double BaseHeight16By10 = 400;
    private const double BaseHeight32By9 = 180;
    private const double AspectRatio4By3 = BaseWidth / BaseHeight4By3;
    private const double AspectRatio16By9 = BaseWidth / BaseHeight16By9;
    private const double AspectRatio16By10 = BaseWidth / BaseHeight16By10;
    private const double AspectRatio32By9 = BaseWidth / BaseHeight32By9;
    private const double ScaleFactor4By3 = 1 / BaseHeight4By3;
    private const double ScaleFactor16By9 = 1 / BaseHeight16By9;
    private const double ScaleFactor16By10 = 1 / BaseHeight16By10;
    private const double ScaleFactor32By9 = 1 / BaseHeight32By9;

    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public float ScaleFactor { get; private set; }
    public int Padding { get; private set; }
    public int FontSize { get; private set; }
    public int FontSpacing { get; private set; }

    public void Update()
    {
        if (WindowWidth != Raylib.GetRenderWidth() || WindowHeight != Raylib.GetRenderHeight())
        {
            Console.WriteLine("Game resolution has changed, recalculating scaling factors");
            UpdateScaling();
        }
    }

    /// <summary>
    ///     Recalculates scaling variables
    /// </summary>
    private void UpdateScaling()
    {
        WindowWidth = Raylib.GetRenderWidth();
        WindowHeight = Raylib.GetRenderHeight();
        ScaleFactor = GetScaleFactor();
        Padding = (int)(1 * ScaleFactor);
        FontSize = (int)(5 * ScaleFactor);
        FontSpacing = (int)(2 * ScaleFactor);

        ServiceManager.GetService<GameUi>().UpdateScaling();
        ServiceManager.GetService<GameSettingsUi>().UpdateScaling();
        ServiceManager.GetService<MainMenuUi>().UpdateScaling();
        ServiceManager.GetService<NewGameMenuUi>().UpdateScaling();

        LabelButton.UpdateScaling();
        SelectedEntityMenu.UpdateScaling();
        TextField.UpdateScaling();
        ToolTip.UpdateScaling();
    }

    /// <summary>
    ///     Calculates a vertical scaling factor for the current game resolution
    /// </summary>
    /// <returns> Float that represents the vertical scaling factor </returns>
    private float GetScaleFactor()
    {
        return ((double)Raylib.GetRenderWidth() / Raylib.GetRenderHeight()) switch
        {
            AspectRatio4By3 => (float)Math.Round(Raylib.GetRenderHeight() * ScaleFactor4By3, 2),
            AspectRatio16By9 => (float)Math.Round(Raylib.GetRenderHeight() * ScaleFactor16By9, 2),
            AspectRatio16By10 => (float)Math.Round(Raylib.GetRenderHeight() * ScaleFactor16By10, 2),
            _ => (float)Math.Round(Raylib.GetRenderHeight() * ScaleFactor4By3, 2)
        };
    }
}