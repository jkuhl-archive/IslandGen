using System.Numerics;
using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.Objects.UI;
using IslandGen.Utils;
using Raylib_CsLo;

namespace IslandGen.Services;

public class MainMenuUi
{
    private const int ButtonWidth = 80;
    private const int ButtonHeight = 40;
    private const float BackgroundTileScale = 4.0f;
    private const string GameTitle = "IslandGen";
    private const string GameMajorVersion = "0.1-alpha";

    private readonly List<TextureButton> _buttonsList;
    private readonly Texture _sandTexture = Assets.Textures[TileType.Sand.GetTileTextureName()];
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
        _buttonsList = new List<TextureButton>
        {
            new(
                Assets.Textures["buttons/new_island"],
                () => ServiceManager.GetService<StateManager>().NewGameMenu(),
                toolTip: new List<string> { "Generate a new island" }),
            new(
                Assets.Textures["buttons/load_island"],
                SaveUtils.LoadGame,
                toolTip: new List<string> { "Load a saved island" },
                disabled: !SaveUtils.SaveFileExists()),
            new(
                Assets.Textures["buttons/settings"],
                () => ServiceManager.GetService<GameSettingsUi>().ToggleSettingsMenu(),
                toolTip: new List<string> { "Modify game settings" }),
            new(
                Assets.Textures["buttons/exit"],
                Raylib.CloseWindow,
                toolTip: new List<string> { "Exit to desktop" })
        };

        var buildId = File.ReadAllText("assets/build_id.txt").Replace("\n", "");
        _versionString = $"{GameMajorVersion}-{buildId}";
    }

    public void Draw()
    {
        Raylib.DrawTextureTiled(_sandTexture,
            new Rectangle(0, 0, _sandTexture.width, _sandTexture.height),
            _backgroundArea,
            Vector2.Zero,
            0,
            BackgroundTileScale,
            Raylib.WHITE);

        Raylib.DrawTextEx(Raylib.GetFontDefault(), GameTitle, _titleArea.Start() + Vector2.One * 4, _titleFontSize,
            _titleFontSpacing, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), GameTitle, _titleArea.Start(), _titleFontSize, _titleFontSpacing,
            Raylib.WHITE);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _versionString, _versionArea.Start() + Vector2.One * 2,
            _versionFontSize, _versionFontSpacing, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _versionString, _versionArea.Start(), _versionFontSize,
            _versionFontSpacing, Raylib.WHITE);

        // Draw buttons in reverse order to tooltips don't draw behind other buttons
        foreach (var button in _buttonsList.GetReverse()) button.Draw();
    }

    public void Update()
    {
        foreach (var button in _buttonsList) button.Update();
    }

    /// <summary>
    ///     Updates the load game button based on if there is a save file to be loaded
    /// </summary>
    public void UpdateLoadGameButton()
    {
        _buttonsList[1].Disabled = !SaveUtils.SaveFileExists();
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _titleFontSize = scalingManager.FontSize * 8;
        _titleFontSpacing = scalingManager.FontSpacing * 2;
        _versionFontSize = scalingManager.FontSize * 2;
        _versionFontSpacing = scalingManager.FontSpacing;
        var windowWidthCenter = scalingManager.WindowWidth / 2;
        var windowHeightCenter = scalingManager.WindowHeight / 2;
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
            _buttonsList[i].SetArea(_buttonsArea with
            {
                Y = _buttonsArea.Y + i * (ButtonHeight * scalingManager.ScaleFactor + scalingManager.Padding),
                height = ButtonHeight * scalingManager.ScaleFactor
            });
    }
}