using System.Numerics;
using IslandGen.Data;
using IslandGen.Objects.ECS.Entities;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.UI;

public static class SelectedEntityMenu
{
    private const int ButtonSize = 25;
    private const int MenuWidth = 250;
    private const int MenuHeight = 80;

    private static int _fontSize;
    private static int _fontSpacing;
    private static int _padding;
    private static int _scaledButtonSize;
    private static Rectangle _innerArea;
    private static Vector2 _infoStart;
    private static readonly List<TextureButton> Buttons = new();

    private static readonly TextureButton DeconstructButton = new(
        Assets.Textures["buttons/deconstruct"],
        DeconstructAction,
        toolTip: new List<string>
        {
            "Deconstruct this",
            "Resources are refunded"
        });

    private static readonly TextureButton RenameButton = new(
        Assets.Textures["buttons/rename"],
        RenameAction,
        toolTip: new List<string>
        {
            "Rename this"
        });

    private static string _info = string.Empty;
    public static Rectangle Area { get; private set; }

    public static void Draw()
    {
        if (ServiceManager.GetService<GameLogic>().SelectedEntity == null) return;

        Raylib.DrawRectangleRec(Area, Raylib.WHITE);
        Raylib.DrawRectangleRec(_innerArea, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), _info, _infoStart, _fontSize, _fontSpacing, Raylib.WHITE);
        foreach (var button in Buttons) button.Draw();
    }

    public static void Update()
    {
        var selectedEntity = ServiceManager.GetService<GameLogic>().SelectedEntity;
        if (selectedEntity == null) return;

        _info = selectedEntity.GetInfoString();
        foreach (var button in Buttons) button.Update();
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public static void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var gameUi = ServiceManager.GetService<GameUi>();
        _fontSize = scalingManager.FontSize;
        _fontSpacing = scalingManager.FontSpacing;
        _padding = scalingManager.Padding;
        _scaledButtonSize = ButtonSize * (int)scalingManager.ScaleFactor;

        Area = new Rectangle(
            (scalingManager.WindowWidth - gameUi.SidebarArea.width - MenuWidth * scalingManager.ScaleFactor) / 2,
            scalingManager.WindowHeight - MenuHeight * scalingManager.ScaleFactor,
            (int)(MenuWidth * scalingManager.ScaleFactor),
            (int)(MenuHeight * scalingManager.ScaleFactor));
        _innerArea = new Rectangle
        (
            Area.X + _padding,
            Area.Y + _padding,
            Area.width - _padding * 2,
            Area.height - _padding * 2
        );
        _infoStart = new Vector2(Area.X + _padding * 4, Area.Y + _padding * 4);
        UpdateButtonPositions();
    }

    /// <summary>
    ///     Refreshes the buttons available in the menu for the current selected entity
    /// </summary>
    public static void SetEntityButtons()
    {
        var selectedEntity = ServiceManager.GetService<GameLogic>().SelectedEntity;
        if (selectedEntity == null) return;

        Buttons.Clear();
        Buttons.Add(RenameButton);

        if (selectedEntity.GetType().BaseType == typeof(StructureBase))
        {
            Buttons.Add(DeconstructButton);
            DeconstructButton.Disabled = false;
        }

        UpdateButtonPositions();
    }

    /// <summary>
    ///     Updates the positions of the buttons in the menu
    /// </summary>
    private static void UpdateButtonPositions()
    {
        var x0 = Area.X + Area.width - _padding * 4 - _scaledButtonSize;
        var y = Area.Y + Area.height - _padding * 4 - _scaledButtonSize;

        for (var i = 0; i < Buttons.Count; i++)
            Buttons[i].SetArea(new Rectangle(
                x0 - (_scaledButtonSize + _padding) * i,
                y,
                _scaledButtonSize,
                _scaledButtonSize));
    }

    /// <summary>
    ///     Action triggered by the deconstruct button
    /// </summary>
    private static void DeconstructAction()
    {
        var selectedEntity = ServiceManager.GetService<GameLogic>().SelectedEntity;
        if (selectedEntity == null) return;

        ((StructureBase)selectedEntity).StartDeconstruction();
        DeconstructButton.Disabled = true;
    }

    /// <summary>
    ///     Action triggered by the rename button
    /// </summary>
    private static void RenameAction()
    {
        Console.WriteLine("Rename clicked");
    }
}