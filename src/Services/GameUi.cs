using System.Numerics;
using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.Objects;
using IslandGen.Objects.ECS.Entities;
using IslandGen.Objects.ECS.Entities.Creatures;
using IslandGen.Objects.ECS.Entities.Structures;
using IslandGen.Objects.Textures;
using IslandGen.Objects.UI;
using IslandGen.Utils;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameUi
{
    private const int CalendarWidth = 125;
    private const int CalendarHeight = 20;
    private const int MiniMapSize = 100;
    private const int SidebarButtonWidth = 50;
    private const int SidebarButtonHeight = 20;
    private const int SidebarTabWidth = 25;
    private const int SidebarTabHeight = 10;
    private const int SidebarWidth = 100;
    private const int SidebarHeight = 250;
    private const int SpeedControlsButtonWidth = 25;
    private const int SpeedControlsWidth = SpeedControlsButtonWidth * 3;
    private const int SpeedControlsHeight = 15;
    private const int StatusButtonHeight = 8;
    private const string PausedString = "Paused";

    private readonly TextureButton _buildFarmButton;
    private readonly TextureButton _buildFishingSpotButton;
    private readonly TextureButton _buildLumberYardButton;
    private readonly TextureButton _buildShelterButton;
    private readonly List<TextureButton> _buildTabButtons;
    private readonly TextureButton _buildWellButton;
    private readonly List<string> _itemsList;
    private readonly RenderTexturePro _miniMapTexture;
    private readonly List<TextureButton> _speedControlButtons;
    private readonly List<LabelButton> _statusButtons;
    private readonly List<LabelButton> _systemTabButtons;
    private readonly List<LabelButton> _tabButtons;
    private string _calendarString;
    private GameUiTab _currentUiTab;
    private string _debugInfoString;
    private int _itemsFontSize;
    private int _itemsFontSpacing;
    private Rectangle _itemsStringsArea;
    private Rectangle _sidebarContentsArea;
    private Rectangle _sidebarInnerArea;
    private Rectangle _sidebarTabsArea;
    private int _statusButtonHeight;

    /// <summary>
    ///     Service that manages the game's UI
    /// </summary>
    public GameUi()
    {
        _buildFarmButton = new TextureButton(
            Assets.Textures["buttons/build_farm"],
            () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Farm()),
            toolTip: new List<string>
            {
                Farm.Description, $"Requirements: {Farm.Requirements}", $"Cost: {CostUtils.GetCostString(Farm.Cost)}"
            });
        _buildFishingSpotButton = new TextureButton(
            Assets.Textures["buttons/build_fishing_spot"],
            () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new FishingSpot()),
            toolTip: new List<string>
            {
                FishingSpot.Description, $"Requirements: {FishingSpot.Requirements}",
                $"Cost: {CostUtils.GetCostString(FishingSpot.Cost)}"
            });
        _buildLumberYardButton = new TextureButton(
            Assets.Textures["buttons/build_lumber_yard"],
            () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new LumberYard()),
            toolTip: new List<string>
            {
                LumberYard.Description, $"Requirements: {LumberYard.Requirements}",
                $"Cost: {CostUtils.GetCostString(LumberYard.Cost)}"
            });
        _buildShelterButton = new TextureButton(
            Assets.Textures["buttons/build_shelter"],
            () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Shelter()),
            toolTip: new List<string>
            {
                Shelter.Description, $"Requirements: {Shelter.Requirements}",
                $"Cost: {CostUtils.GetCostString(Shelter.Cost)}"
            });

        _buildWellButton = new TextureButton(
            Assets.Textures["buttons/build_well"],
            () => ServiceManager.GetService<GameLogic>().SetMouseStructure(new Well()),
            toolTip: new List<string>
            {
                Well.Description, $"Requirements: {Well.Requirements}", $"Cost: {CostUtils.GetCostString(Well.Cost)}"
            });
        _buildTabButtons = new List<TextureButton>
        {
            _buildShelterButton,
            _buildLumberYardButton,
            _buildFarmButton,
            _buildWellButton,
            _buildFishingSpotButton
        };
        _itemsList = new List<string>();
        _speedControlButtons = new List<TextureButton>
        {
            new(
                Assets.Textures["buttons/speed_rw"],
                () => ServiceManager.GetService<GameLogic>().DecreaseGameSpeed(),
                toolTip: new List<string> { "Decrease game speed" }),
            new(
                Assets.Textures["buttons/speed_pause"],
                PauseGame,
                toolTip: new List<string> { "Toggle pausing the game" }),
            new(
                Assets.Textures["buttons/speed_ff"],
                () => ServiceManager.GetService<GameLogic>().IncreaseGameSpeed(),
                toolTip: new List<string> { "Increase game speed" })
        };
        _statusButtons = new List<LabelButton>();
        _systemTabButtons = new List<LabelButton>
        {
            new("Save Island", SaveUtils.SaveGame),
            new("Load Island", SaveUtils.LoadGame),
            new("Settings", () => ServiceManager.GetService<GameSettingsUi>().ToggleSettingsMenu()),
            new("Main Menu", () => ServiceManager.GetService<StateManager>().MainMenu())
        };
        _tabButtons = new List<LabelButton>
        {
            new("Build", () => _currentUiTab = GameUiTab.Build),
            new("Status", () => _currentUiTab = GameUiTab.Status),
            new("Items", () => _currentUiTab = GameUiTab.Items),
            new("System", () => _currentUiTab = GameUiTab.System)
        };

        _calendarString = string.Empty;
        _currentUiTab = GameUiTab.Build;
        _debugInfoString = string.Empty;
        _miniMapTexture = new RenderTexturePro((MiniMapSize, MiniMapSize));
    }

    public Rectangle CalendarArea { get; private set; }
    public Rectangle MiniMapArea { get; private set; }
    public Rectangle SidebarArea { get; private set; }
    public Rectangle SpeedControlsArea { get; private set; }

    public void Draw()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Draw calendar
        DrawPopUp(_calendarString, CalendarArea);

        // Draw selected entity menu
        gameLogic.SelectedEntity?.DrawSelectedMenu();

        // Start minimap texture rendering
        Raylib.BeginTextureMode(_miniMapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);

        // Render map to minimap texture
        for (var mapX = 0; mapX < GameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < GameMap.MapSize; mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), gameLogic.GameMap.GetTileType((mapX, mapY)).GetTileColor());

        // Render structures to minimap texture
        foreach (var structure in gameLogic.GetEntityBaseTypeList<StructureBase>())
        foreach (var tile in structure.GetOccupiedTiles())
            Raylib.DrawPixelV(new Vector2(tile.Item1, tile.Item2), structure.MiniMapColor);

        // Render colonists to minimap texture
        foreach (var colonist in gameLogic.GetEntityList<Colonist>())
            Raylib.DrawPixelV(new Vector2(colonist.MapPosition.Item1, colonist.MapPosition.Item2),
                colonist.MiniMapColor);

        // Draw box on minimap texture that represents the area of the map currently visible
        Raylib.DrawRectangleLinesEx(gameLogic.GameMap.GetVisibleMapTiles(), 1, Colors.MiniMapViewBox);

        // End minimap texture rendering
        Raylib.EndTextureMode();

        // Draw sidebar backdrop
        Raylib.DrawRectangleRec(SidebarArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(_sidebarContentsArea, Raylib.GRAY);
        Raylib.DrawRectangleRec(_sidebarTabsArea, Raylib.DARKGRAY);

        // Draw minimap
        _miniMapTexture.Draw();

        // Draw tab contents
        switch (_currentUiTab)
        {
            case GameUiTab.Build:
                // Row 3
                _buildFishingSpotButton.Draw();

                // Row 2
                _buildFarmButton.Draw();
                _buildWellButton.Draw();

                // Row 1
                _buildShelterButton.Draw();
                _buildLumberYardButton.Draw();
                break;
            case GameUiTab.Items:
                for (var i = 0; i < _itemsList.Count; i++)
                    Raylib.DrawTextEx(Raylib.GetFontDefault(),
                        _itemsList[i],
                        new Vector2(_itemsStringsArea.X, _itemsStringsArea.Y + i * _itemsFontSize),
                        _itemsFontSize,
                        _itemsFontSpacing,
                        Raylib.WHITE);
                break;
            case GameUiTab.Status:
                foreach (var button in _statusButtons) button.Draw();
                break;
            case GameUiTab.System:
                foreach (var button in _systemTabButtons) button.Draw();
                break;
        }

        // Draw tab buttons
        foreach (var button in _tabButtons) button.Draw();

        // Draw speed controls
        foreach (var button in _speedControlButtons) button.Draw();

        // Draw debug info
        if (gameSettings.DebugMode) DrawPopUp(_debugInfoString, new Rectangle(), true);
    }

    public void Update()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Set calendar string
        _calendarString = $"{gameLogic.CurrentDateTime.ToLongDateString()}\n" +
                          $"{gameLogic.CurrentDateTime.ToLongTimeString()}";

        // Append paused string if game is paused
        if (gameLogic.GamePaused)
            _calendarString += $" - {PausedString}";
        else
            _calendarString += $" - {gameLogic.GameSpeed}";

        // Update selected entity menu
        gameLogic.SelectedEntity?.UpdateSelectedMenu();

        // Set tab contents
        switch (_currentUiTab)
        {
            case GameUiTab.Status:
                _statusButtons.Clear();
                var colonistList = gameLogic.GetEntityList<Colonist>();
                for (var i = 0; i < colonistList.Count; i++)
                {
                    var colonist = colonistList[i];
                    _statusButtons.Add(new LabelButton(
                        $"{colonist.ReadableName} - {colonist.GetRoutineStatus()}",
                        () =>
                        {
                            gameLogic.SetSelectedEntity(colonist);
                            gameLogic.GameCamera.LookAtTile(colonist.MapPosition);
                        },
                        _sidebarContentsArea with
                        {
                            Y = _sidebarContentsArea.Y + i * _statusButtonHeight,
                            height = _statusButtonHeight
                        }));
                }

                break;

            case GameUiTab.Items:
                _itemsList.Clear();
                foreach (var resource in gameLogic.GetResourceCounts())
                    _itemsList.Add($"{resource.Key.GetResourceName()}: {resource.Value}");
                break;
        }

        // Update speed controls
        foreach (var button in _speedControlButtons) button.Update();

        // Update tab buttons
        foreach (var button in _tabButtons) button.Update();

        // Update tab buttons
        switch (_currentUiTab)
        {
            case GameUiTab.Build:
                _buildShelterButton.Disabled = !CostUtils.CanAfford(Shelter.Cost);
                _buildLumberYardButton.Disabled = !CostUtils.CanAfford(LumberYard.Cost);
                _buildWellButton.Disabled = !CostUtils.CanAfford(Well.Cost);
                _buildFarmButton.Disabled = !CostUtils.CanAfford(Farm.Cost);
                _buildFishingSpotButton.Disabled = !CostUtils.CanAfford(FishingSpot.Cost);
                foreach (var button in _buildTabButtons) button.Update();
                break;
            case GameUiTab.Status:
                foreach (var button in _statusButtons) button.Update();
                break;
            case GameUiTab.System:
                foreach (var button in _systemTabButtons) button.Update();
                break;
        }

        // Set debug info string
        if (gameSettings.DebugMode)
        {
            var scalingManager = ServiceManager.GetService<ScalingManager>();
            _debugInfoString =
                $"FPS: {Raylib.GetFPS()}\n" +
                $"Window Resolution: {scalingManager.WindowWidth}x{scalingManager.WindowHeight}\n" +
                $"Scaling Factor: {scalingManager.ScaleFactor}\n" +
                "\n" +
                $"Mouse Window Position: {InputManager.GetMousePosition()}\n" +
                $"Mouse Map Position: {gameLogic.GameMap.GetMapMousePosition()}\n" +
                $"Mouse Highlighted Tile: {gameLogic.GameMap.GetMapMouseTile()}\n" +
                "\n" +
                $"Camera Zoom: {gameLogic.GameCamera.Camera.zoom}x\n" +
                $"Camera Target: {gameLogic.GameCamera.Camera.target}\n" +
                $"Camera Pan Limits: {gameLogic.GameMap.GetCameraPanLimits()}\n" +
                $"Camera Visible Map Tiles: {gameLogic.GameMap.GetVisibleMapTiles().String()}";
        }
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _itemsFontSize = scalingManager.FontSize;
        _itemsFontSpacing = scalingManager.FontSpacing;
        _statusButtonHeight = (int)(StatusButtonHeight * scalingManager.ScaleFactor);

        // Set calendar area
        CalendarArea = new Rectangle(
            scalingManager.WindowWidth - CalendarWidth * scalingManager.ScaleFactor,
            0,
            (int)(CalendarWidth * scalingManager.ScaleFactor),
            (int)(CalendarHeight * scalingManager.ScaleFactor));

        // Set sidebar area
        SidebarArea = new Rectangle(
            scalingManager.WindowWidth - SidebarWidth * scalingManager.ScaleFactor - scalingManager.Padding * 2,
            scalingManager.WindowHeight - SidebarHeight * scalingManager.ScaleFactor - scalingManager.Padding * 4,
            (int)(SidebarWidth * scalingManager.ScaleFactor + scalingManager.Padding * 2),
            (int)(SidebarHeight * scalingManager.ScaleFactor + scalingManager.Padding * 4));

        // Set sidebar inner area
        _sidebarInnerArea = new Rectangle(
            SidebarArea.X + scalingManager.Padding,
            SidebarArea.Y + scalingManager.Padding,
            SidebarWidth * scalingManager.ScaleFactor,
            SidebarHeight * scalingManager.ScaleFactor + scalingManager.Padding * 2);

        // Set minimap area
        MiniMapArea = new Rectangle(
            _sidebarInnerArea.X,
            _sidebarInnerArea.Y + _sidebarInnerArea.height - MiniMapSize * scalingManager.ScaleFactor,
            (int)(MiniMapSize * scalingManager.ScaleFactor),
            (int)(MiniMapSize * scalingManager.ScaleFactor));

        // Set sidebar tabs area
        _sidebarTabsArea = new Rectangle(
            _sidebarInnerArea.X,
            _sidebarInnerArea.Y,
            _sidebarInnerArea.width,
            SidebarTabHeight * scalingManager.ScaleFactor);

        // Set sidebar contents area
        _sidebarContentsArea = new Rectangle(
            _sidebarInnerArea.X,
            _sidebarTabsArea.Y + _sidebarTabsArea.height + scalingManager.Padding,
            _sidebarInnerArea.width,
            _sidebarInnerArea.height - _sidebarTabsArea.height - MiniMapArea.height - scalingManager.Padding * 2);

        // Set items strings area
        _itemsStringsArea = new Rectangle(
            _sidebarContentsArea.X + scalingManager.Padding,
            _sidebarContentsArea.Y + scalingManager.Padding,
            _sidebarContentsArea.width - scalingManager.Padding * 2,
            _sidebarContentsArea.height - scalingManager.Padding * 2);

        // Set speed controls area
        SpeedControlsArea = new Rectangle(
            SidebarArea.X + (SidebarArea.width - SpeedControlsWidth * scalingManager.ScaleFactor) / 2,
            SidebarArea.y - SpeedControlsHeight * scalingManager.ScaleFactor - scalingManager.Padding,
            (int)(SpeedControlsWidth * scalingManager.ScaleFactor),
            (int)(SpeedControlsHeight * scalingManager.ScaleFactor)
        );

        // Set tab button positions
        for (var i = 0; i < _tabButtons.Count; i++)
            _tabButtons[i].SetArea(new Rectangle(
                _sidebarTabsArea.X + i * SidebarTabWidth * scalingManager.ScaleFactor,
                _sidebarTabsArea.Y,
                SidebarTabWidth * scalingManager.ScaleFactor,
                SidebarTabHeight * scalingManager.ScaleFactor));

        // Set sidebar button positions
        var columnCounter = 0;
        var rowCounter = 0;
        for (var i = 0; i < 20; i++)
        {
            var buttonArea = new Rectangle(
                _sidebarContentsArea.X + columnCounter * SidebarButtonWidth * scalingManager.ScaleFactor,
                _sidebarContentsArea.Y + rowCounter * SidebarButtonHeight * scalingManager.ScaleFactor,
                SidebarButtonWidth * scalingManager.ScaleFactor,
                SidebarButtonHeight * scalingManager.ScaleFactor);

            if (i <= _buildTabButtons.Count - 1) _buildTabButtons[i].SetArea(buttonArea);
            if (i <= _systemTabButtons.Count - 1) _systemTabButtons[i].SetArea(buttonArea);

            columnCounter++;

            if (columnCounter >= 2)
            {
                columnCounter = 0;
                rowCounter++;
            }
        }

        // Set speed controls button positions
        for (var i = 0; i < _speedControlButtons.Count; i++)
            _speedControlButtons[i].SetArea(new Rectangle(
                SpeedControlsArea.X + i * SpeedControlsButtonWidth * scalingManager.ScaleFactor,
                SpeedControlsArea.Y,
                SpeedControlsButtonWidth * scalingManager.ScaleFactor,
                SpeedControlsArea.height));

        // Apply minimap area to texture
        _miniMapTexture.DestinationRectangle = MiniMapArea;
    }

    /// <summary>
    ///     Draws a popup message on the screen
    /// </summary>
    /// <param name="message"> String containing the popup contents </param>
    /// <param name="popupArea"> Rectangle that contains the area on screen the popup should occupy </param>
    /// <param name="scalePopup"> If true popup area will be scaled to fit contents of message </param>
    private void DrawPopUp(string message, Rectangle popupArea, bool scalePopup = false)
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var fontSize = scalingManager.FontSize;
        var fontSpacing = scalingManager.FontSpacing;
        var padding = scalingManager.Padding;

        if (scalePopup)
        {
            var messageSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), message, fontSize, fontSpacing);
            popupArea = popupArea with
            {
                width = messageSize.X_int() + padding * 8,
                height = messageSize.Y_int() + padding * 8
            };
        }

        var innerPopupArea = new Rectangle
        (
            popupArea.X + padding,
            popupArea.Y + padding,
            popupArea.width - padding * 2,
            popupArea.height - padding * 2
        );

        Raylib.DrawRectangleRec(popupArea, Raylib.WHITE);
        Raylib.DrawRectangleRec(innerPopupArea, Raylib.BLACK);
        Raylib.DrawTextEx(Raylib.GetFontDefault(), message,
            new Vector2(popupArea.X + padding * 4, popupArea.Y + padding * 4),
            fontSize, fontSpacing, Raylib.WHITE);
    }

    /// <summary>
    ///     Handles pausing the game and updating the texture for the pause/play button
    /// </summary>
    private void PauseGame()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();

        gameLogic.ToggleGamePaused();
        _speedControlButtons[1].SetTexture(gameLogic.GamePaused
            ? Assets.Textures["buttons/speed_play"]
            : Assets.Textures["buttons/speed_pause"]);
    }
}