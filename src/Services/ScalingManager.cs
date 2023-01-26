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

    private const double WidthScaleFactor = 1 / BaseWidth;
    private const double HeightScaleFactor4By3 = 1 / BaseHeight4By3;
    private const double HeightScaleFactor16By9 = 1 / BaseHeight16By9;
    private const double HeightScaleFactor16By10 = 1 / BaseHeight16By10;
    private const double HeightScaleFactor32By9 = 1 / BaseHeight32By9;

    /// <summary>
    ///     Service that manages automatically scaling the game to fit the current window size / resolution
    /// </summary>
    public ScalingManager()
    {
        UpdateScaling();
    }

    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public float WidthScale { get; private set; }
    public float HeightScale { get; private set; }
    public int WidthPadding { get; private set; }
    public int HeightPadding { get; private set; }
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
    ///     Recalculates scaling factors
    /// </summary>
    private void UpdateScaling()
    {
        WindowWidth = Raylib.GetRenderWidth();
        WindowHeight = Raylib.GetRenderHeight();
        WidthScale = GetWidthScale();
        HeightScale = GetHeightScale();
        WidthPadding = (int)(1 * WidthScale);
        HeightPadding = (int)(1 * HeightScale);
        FontSize = (int)((WidthScale + HeightScale) / 2 * 10);
        FontSpacing = (int)(2 * WidthScale);
    }

    /// <summary>
    ///     Calculates a vertical scaling factor for the current game resolution
    /// </summary>
    /// <returns> Float that represents the vertical scaling factor </returns>
    private float GetHeightScale()
    {
        return ((double)Raylib.GetRenderWidth() / Raylib.GetRenderHeight()) switch
        {
            AspectRatio4By3 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor4By3, 2),
            AspectRatio16By9 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor16By9, 2),
            AspectRatio16By10 => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor16By10, 2),
            _ => (float)Math.Round(Raylib.GetRenderHeight() * HeightScaleFactor4By3, 2)
        };
    }

    /// <summary>
    ///     Calculates a horizontal scaling factor for the current game resolution
    /// </summary>
    /// <returns> Float that represents the horizontal scaling factor </returns>
    private float GetWidthScale()
    {
        return (float)Math.Round(Raylib.GetRenderWidth() * WidthScaleFactor, 2);
    }
}