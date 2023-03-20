using IslandGen.Data;
using IslandGen.Extensions;
using IslandGen.Objects.UI;
using IslandGen.Utils;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameSettingsUi
{
    private const int ButtonHeight = 25;
    private const int ExitButtonSize = 12;
    private const int SettingsMenuWidth = 150;
    private const int SettingsMenuHeight = 200;
    private const string SettingsMenuTitle = "Settings";

    private readonly TextureButton _debugModeButton;
    private readonly TextureButton _exitButton;
    private readonly TextureButton _fullscreenButton;
    private readonly List<TextureButton> _settingsButtonsList;
    private Rectangle _buttonsArea;
    private Rectangle _menuBackdrop;
    private Rectangle _menuInnerBackdrop;
    private Rectangle _menuShade;
    private Rectangle _titleArea;
    private int _titleFontSize;
    private int _titleFontSpacing;

    public GameSettingsUi()
    {
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Initialize buttons
        _debugModeButton = new TextureButton(
            Assets.Textures["buttons/settings_debug_disabled"],
            ToggleDebugMode,
            toolTip: new List<string> { "Toggle debug mode" },
            settingsButton: true);
        _exitButton = new TextureButton(
            Assets.Textures["buttons/settings_exit"],
            ToggleSettingsMenu,
            settingsButton: true);
        _fullscreenButton = new TextureButton(
            Assets.Textures["buttons/settings_fullscreen"],
            ToggleFullscreen,
            toolTip: new List<string> { "Toggle fullscreen" },
            settingsButton: true);

        // Update button labels to match settings state
        if (gameSettings.DebugMode) _debugModeButton.SetTexture(Assets.Textures["buttons/settings_debug_enabled"]);
        if (gameSettings.Fullscreen) _fullscreenButton.SetTexture(Assets.Textures["buttons/settings_windowed"]);

        // Add buttons to a list so they can be iterated over
        _settingsButtonsList = new List<TextureButton>
        {
            _fullscreenButton,
            _debugModeButton
        };
    }

    public bool SettingsMenuActive { get; private set; }

    public void Draw()
    {
        if (!SettingsMenuActive) return;

        Raylib.DrawRectangleRec(_menuShade, Colors.SettingsMenuShade);
        Raylib.DrawRectangleRec(_menuBackdrop, Raylib.WHITE);
        Raylib.DrawRectangleRec(_menuInnerBackdrop, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), SettingsMenuTitle, _titleArea.Start(), _titleFontSize,
            _titleFontSpacing, Raylib.WHITE);
        _exitButton.Draw();

        // Draw buttons in reverse order to tooltips don't draw behind other buttons
        foreach (var button in _settingsButtonsList) button.Draw();
    }

    public void Update()
    {
        if (!SettingsMenuActive) return;

        _exitButton.Update();
        foreach (var button in _settingsButtonsList) button.Update();
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _titleFontSize = scalingManager.FontSize * 3;
        _titleFontSpacing = scalingManager.FontSpacing;
        var windowWidthCenter = scalingManager.WindowWidth / 2;
        var windowHeightCenter = scalingManager.WindowHeight / 2;
        var titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), SettingsMenuTitle,
            _titleFontSize, _titleFontSpacing);

        // Set backdrop rectangles
        _menuBackdrop = new Rectangle(
            windowWidthCenter - SettingsMenuWidth * scalingManager.ScaleFactor / 2,
            windowHeightCenter - SettingsMenuHeight * scalingManager.ScaleFactor / 2,
            SettingsMenuWidth * scalingManager.ScaleFactor,
            SettingsMenuHeight * scalingManager.ScaleFactor);
        _menuInnerBackdrop = new Rectangle(
            _menuBackdrop.X + scalingManager.Padding,
            _menuBackdrop.Y + scalingManager.Padding,
            _menuBackdrop.width - scalingManager.Padding * 2,
            _menuBackdrop.height - scalingManager.Padding * 2);
        _menuShade = new Rectangle(
            0,
            0,
            scalingManager.WindowWidth,
            scalingManager.WindowHeight);

        // Set title area
        _titleArea = new Rectangle(
            _menuInnerBackdrop.X + scalingManager.Padding * 2,
            _menuInnerBackdrop.Y + scalingManager.Padding,
            titleSize.X,
            titleSize.Y + titleSize.Y / 2);

        // Set exit button area
        _exitButton.SetArea(new Rectangle(
            _menuInnerBackdrop.X + _menuInnerBackdrop.width - ExitButtonSize * scalingManager.ScaleFactor -
            scalingManager.Padding * 2,
            _menuInnerBackdrop.Y + scalingManager.Padding * 2,
            ExitButtonSize * scalingManager.ScaleFactor,
            ExitButtonSize * scalingManager.ScaleFactor));

        // Set buttons area
        _buttonsArea = new Rectangle(
            _menuInnerBackdrop.X + scalingManager.Padding * 2,
            _titleArea.Y + _titleArea.height,
            _menuInnerBackdrop.width - scalingManager.Padding * 4,
            _menuInnerBackdrop.height - _titleArea.height - scalingManager.Padding * 3);

        // Set area for each button in the buttons list
        for (var i = 0; i < _settingsButtonsList.Count; i++)
            _settingsButtonsList[i].SetArea(_buttonsArea with
            {
                Y = _buttonsArea.Y + i * (ButtonHeight * scalingManager.ScaleFactor + scalingManager.Padding),
                height = ButtonHeight * scalingManager.ScaleFactor
            });
    }

    /// <summary>
    ///     Toggles displaying the settings menu
    /// </summary>
    public void ToggleSettingsMenu()
    {
        SaveUtils.SaveSettings();
        SettingsMenuActive = !SettingsMenuActive;
    }

    /// <summary>
    ///     Toggles debug mode
    /// </summary>
    private void ToggleDebugMode()
    {
        var gameSettings = ServiceManager.GetService<GameSettings>();

        gameSettings.DebugMode = !gameSettings.DebugMode;
        _debugModeButton.SetTexture(gameSettings.DebugMode
            ? Assets.Textures["buttons/settings_debug_enabled"]
            : Assets.Textures["buttons/settings_debug_disabled"]);
    }

    /// <summary>
    ///     Toggles fullscreen / windowed mode
    /// </summary>
    private void ToggleFullscreen()
    {
        var gameSettings = ServiceManager.GetService<GameSettings>();

        gameSettings.Fullscreen = !gameSettings.Fullscreen;
        if (gameSettings.Fullscreen != Raylib.IsWindowFullscreen()) Raylib.ToggleFullscreen();
        _fullscreenButton.SetTexture(gameSettings.Fullscreen
            ? Assets.Textures["buttons/settings_windowed"]
            : Assets.Textures["buttons/settings_fullscreen"]);
    }
}