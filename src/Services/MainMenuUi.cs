using System.Numerics;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.UI;
using Raylib_CsLo;

namespace IslandGen.Services;

public class MainMenuUi
{
    private const int ButtonWidth = 80;
    private const int ButtonHeight = 40;
    private const float BackgroundTileScale = 4.0f;
    private const string GameTitle = "IslandGen";
    private const string GameMajorVersion = "0.1-alpha";

    private readonly List<Button> _buttonsList;
    private readonly string _versionString;

    private Rectangle _backgroundArea;
    private Rectangle _buttonsArea;
    private Rectangle _titleArea;
    private int _titleFontSize;
    private int _titleFontSpacing;
    private Rectangle _versionArea;
    private int _versionFontSize;
    private int _versionFontSpacing;

    /// <summary>
    ///     Service that provides a main menu interface for accessing the game
    /// </summary>
    public MainMenuUi()
    {
        _buttonsList = new List<Button>
        {
            new("New Island", StateManager.NewGame),
            new("Load Island", StateManager.LoadGame),
            new("Fullscreen", Raylib.ToggleFullscreen),
            new("Exit Game", Raylib.CloseWindow)
        };

        var buildId = File.ReadAllText("assets/build_id.txt").Replace("\n", "");
        _versionString = $"{GameMajorVersion}-{buildId}";
    }

    public void Draw()
    {
        var texture = ServiceManager.GetService<TextureManager>().Textures[TileType.Dirt.GetTileTextureName()];
        Raylib.DrawTextureTiled(texture, new Rectangle(0, 0, texture.width, texture.height), _backgroundArea,
            Vector2.Zero, 0, BackgroundTileScale, Raylib.WHITE);

        Raylib.DrawTextEx(Raylib.GetFontDefault(), GameTitle, _titleArea.Start() + Vector2.One * 4, _titleFontSize,
            _titleFontSpacing, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), GameTitle, _titleArea.Start(), _titleFontSize, _titleFontSpacing,
            Raylib.WHITE);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _versionString, _versionArea.Start() + Vector2.One * 2,
            _versionFontSize, _versionFontSpacing, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _versionString, _versionArea.Start(), _versionFontSize,
            _versionFontSpacing, Raylib.WHITE);


        foreach (var button in _buttonsList) button.Draw();
    }

    public void Update()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var windowWidthCenter = scalingManager.WindowWidth / 2;
        var windowHeightCenter = scalingManager.WindowHeight / 2;
        _titleFontSize = scalingManager.FontSize * 8;
        _titleFontSpacing = scalingManager.FontSpacing * 2;
        _versionFontSize = scalingManager.FontSize * 2;
        _versionFontSpacing = scalingManager.FontSpacing;

        var buttonsAreaHeight =
            (ButtonHeight + scalingManager.Padding) * scalingManager.ScaleFactor * _buttonsList.Count -
            scalingManager.Padding;
        var titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), GameTitle, _titleFontSize, _titleFontSpacing);
        var versionSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), _versionString, _versionFontSize,
            _versionFontSpacing);

        _backgroundArea = new Rectangle(0, 0, scalingManager.WindowWidth, scalingManager.WindowHeight);
        _versionArea = new Rectangle(
            scalingManager.WindowWidth - versionSize.X - scalingManager.Padding,
            scalingManager.WindowHeight - versionSize.Y - scalingManager.Padding,
            versionSize.X,
            versionSize.Y);
        _titleArea = new Rectangle(
            windowWidthCenter - titleSize.X / 2,
            windowHeightCenter - titleSize.Y - buttonsAreaHeight / 2,
            titleSize.X,
            titleSize.Y + titleSize.Y / 2);
        _buttonsArea = new Rectangle(
            windowWidthCenter - ButtonWidth * scalingManager.ScaleFactor / 2,
            _titleArea.Y + _titleArea.height,
            ButtonWidth * scalingManager.ScaleFactor,
            buttonsAreaHeight);

        for (var i = 0; i < _buttonsList.Count; i++)
            _buttonsList[i].Area = _buttonsArea with
            {
                Y = _buttonsArea.Y + i * (ButtonHeight * scalingManager.ScaleFactor + scalingManager.Padding),
                height = ButtonHeight * scalingManager.ScaleFactor
            };
    }
}