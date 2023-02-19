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
using Raylib_CsLo;

namespace IslandGen.Services;

public class NewGameMenuUi
{
    private const int ButtonWidth = 50;
    private const int ButtonHeight = 20;
    private const int CrewSize = 10;
    private const int DepartureYearStart = 1603;
    private const int DepartureYearEnd = 1627;
    private const int MapPreviewSize = 100;

    private readonly List<Vector2> _backgroundStarPositions;
    private readonly Button _mainMenuButton;
    private readonly RenderTexturePro _mapPreviewTexture;
    private readonly Button _randomizeButton;
    private readonly Button _startGameButton;
    private Rectangle _background;
    private string _captainName;
    private DateTime _departureDate;
    private GameLogic _gameLogic;
    private Rectangle _mapPreviewArea;
    private Rectangle _menuBackdrop;
    private Rectangle _menuInnerBackdrop;
    private int _menuPadding;
    private int _promptFontSize;
    private int _promptFontSpacing;
    private string _shipName;
    private DateTime _stormDate;

    public NewGameMenuUi()
    {
        _mainMenuButton = new Button("Main Menu",
            () => ServiceManager.GetService<StateManager>().MainMenu());
        _randomizeButton = new Button("Randomize Island", () => _gameLogic!.GameMap.GenerateMap());
        _startGameButton = new Button("Start Game", StartGame);
        _mapPreviewTexture = new RenderTexturePro((MapPreviewSize, MapPreviewSize));

        _backgroundStarPositions = new List<Vector2>();
        _gameLogic = new GameLogic();
        _captainName = string.Empty;
        _shipName = string.Empty;
    }

    public void Draw()
    {
        // Draw background sky
        Raylib.DrawRectangleRec(_background, Colors.NewGameBackground);
        foreach (var star in _backgroundStarPositions) Raylib.DrawPixelV(star, Raylib.WHITE);

        // Draw new game menu backdrop
        Raylib.DrawRectangleRec(_menuBackdrop, Raylib.WHITE);
        Raylib.DrawRectangleRec(_menuInnerBackdrop, Raylib.BLACK);

        // Draw prompt
        var promptStrings = GetPromptString();
        for (var i = 0; i < promptStrings.Count; i++)
        {
            var line = promptStrings[i];
            var promptSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), line, _promptFontSize, _promptFontSpacing);
            var promptX = _menuInnerBackdrop.X + _menuInnerBackdrop.width / 2 - promptSize.X / 2;
            var promptY = _menuInnerBackdrop.Y + _menuPadding + (promptSize.Y + _promptFontSpacing) * i;

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                line,
                new Vector2(promptX, promptY),
                _promptFontSize,
                _promptFontSpacing,
                Raylib.WHITE);
        }

        // Draw map preview
        Raylib.BeginTextureMode(_mapPreviewTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        for (var mapX = 0; mapX < GameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < GameMap.MapSize; mapY++)
            Raylib.DrawPixelV(new Vector2(mapX, mapY), _gameLogic.GameMap.GetTileType((mapX, mapY)).GetTileColor());
        Raylib.EndTextureMode();
        _mapPreviewTexture.Draw();

        // Draw buttons
        _mainMenuButton.Draw();
        _randomizeButton.Draw();
        _startGameButton.Draw();
    }

    /// <summary>
    ///     Recalculates scaled UI elements
    /// </summary>
    public void UpdateScaling()
    {
        var rnd = ServiceManager.GetService<Random>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        _menuPadding = scalingManager.Padding * 10;
        _promptFontSize = scalingManager.FontSize * 2;
        _promptFontSpacing = scalingManager.FontSpacing;
        var backdropWidth = scalingManager.WindowWidth - scalingManager.WindowWidth / 4;
        var backdropHeight = scalingManager.WindowHeight - scalingManager.WindowHeight / 3;
        var windowWidthCenter = scalingManager.WindowWidth / 2;
        var windowHeightCenter = scalingManager.WindowHeight / 2;

        // Set background area
        _background = new Rectangle(
            0,
            0,
            scalingManager.WindowWidth,
            scalingManager.WindowHeight);

        // Set background stars
        for (var i = 0; i < (scalingManager.WindowWidth + scalingManager.WindowHeight) / 10; i++)
            _backgroundStarPositions.Add(new Vector2(rnd.Next(scalingManager.WindowWidth),
                rnd.Next(scalingManager.WindowHeight)));

        // Set backdrop area
        _menuBackdrop = new Rectangle(
            windowWidthCenter - backdropWidth / 2,
            windowHeightCenter - backdropHeight / 2,
            backdropWidth,
            backdropHeight);
        _menuInnerBackdrop = new Rectangle(
            _menuBackdrop.X + scalingManager.Padding,
            _menuBackdrop.Y + scalingManager.Padding,
            _menuBackdrop.width - scalingManager.Padding * 2,
            _menuBackdrop.height - scalingManager.Padding * 2);

        // Set map preview area
        _mapPreviewArea = new Rectangle(
            _menuInnerBackdrop.X + _menuInnerBackdrop.width - MapPreviewSize * scalingManager.ScaleFactor -
            _menuPadding,
            _menuInnerBackdrop.Y + _menuInnerBackdrop.height - MapPreviewSize * scalingManager.ScaleFactor -
            _menuPadding,
            (int)(MapPreviewSize * scalingManager.ScaleFactor),
            (int)(MapPreviewSize * scalingManager.ScaleFactor));

        // Apply map preview area to texture
        _mapPreviewTexture.DestinationRectangle = _mapPreviewArea;

        // Set main menu button area
        _mainMenuButton.Area = new Rectangle(
            scalingManager.WindowWidth - ButtonWidth * scalingManager.ScaleFactor - scalingManager.Padding * 2,
            scalingManager.Padding * 2,
            ButtonWidth * scalingManager.ScaleFactor,
            ButtonHeight * scalingManager.ScaleFactor);

        // Set randomize button area
        _randomizeButton.Area = new Rectangle(
            _mapPreviewArea.X + _mapPreviewArea.width / 2 - ButtonWidth * scalingManager.ScaleFactor / 2,
            _mapPreviewArea.Y - ButtonHeight * scalingManager.ScaleFactor - scalingManager.Padding * 2,
            ButtonWidth * scalingManager.ScaleFactor,
            ButtonHeight * scalingManager.ScaleFactor);

        // Set start game button area
        _startGameButton.Area = new Rectangle(
            _menuInnerBackdrop.X + _menuPadding,
            _menuInnerBackdrop.Y + _menuInnerBackdrop.height - ButtonHeight * scalingManager.ScaleFactor - _menuPadding,
            ButtonWidth * scalingManager.ScaleFactor,
            ButtonHeight * scalingManager.ScaleFactor);
    }

    /// <summary>
    ///     Initializes a new GameLogic object and randomizes values
    /// </summary>
    public void InitializeGameLogic()
    {
        var rnd = ServiceManager.GetService<Random>();

        // Set dates of voyage
        var dateRangeStart = new DateTime(DepartureYearStart, 1, 1);
        var dateRangeEnd = new DateTime(DepartureYearEnd, 12, 31);
        _departureDate = dateRangeStart.AddDays(rnd.Next((dateRangeEnd - dateRangeStart).Days));
        _stormDate = _departureDate.AddDays(rnd.Next(5, 20));

        // Initialize new GameLogic object
        _gameLogic = new GameLogic { StartDateTime = _stormDate };
        _gameLogic.ResetDateTime();
        _gameLogic.GameMap.GenerateMap();

        // Set captain and ship names
        _captainName = Datasets.MaleNames.RandomItem();
        _shipName = Datasets.FemaleNames.RandomItem();
    }

    /// <summary>
    ///     Gets the prompt string
    /// </summary>
    /// <returns> Prompt string containing ship name and captain name </returns>
    private List<string> GetPromptString()
    {
        return new List<string>
        {
            $"The Spanish naval ship 'The {_shipName}' leaves port on {_departureDate:MMMM d, yyyy}.",
            $"On {_stormDate:MMMM d, yyyy} a sudden storm damages the ship beyond repair.",
            $"Captain {_captainName} makes the decision to beach the ship on a small island."
        };
    }

    /// <summary>
    ///     Starts the game
    /// </summary>
    private void StartGame()
    {
        ServiceManager.AddService(_gameLogic);
        PlaceStartingEntities();
        ServiceManager.GetService<StateManager>().InGame();
    }

    /// <summary>
    ///     Places starting entities on the game map
    /// </summary>
    private void PlaceStartingEntities()
    {
        var rnd = ServiceManager.GetService<Random>();
        var wreckage = new Wreckage { ReadableName = $"Wreckage of The {_shipName}" };

        // Place wreckage
        for (var attempt = 0; attempt < GameMap.MapSize * 10; attempt++)
        {
            wreckage.MapPosition = (rnd.Next(GameMap.MapBuffer),
                rnd.Next(GameMap.MapBuffer, GameMap.MapSize - GameMap.MapBuffer));
            if (!wreckage.ValidPlacement()) continue;

            _gameLogic.AddEntity(wreckage);
            break;
        }

        // Place trees
        for (var mapX = 0; mapX < GameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < GameMap.MapSize; mapY++)
        {
            var treeBaseY = mapY + 1;
            if (treeBaseY >= GameMap.MapSize) continue;
            if (
                wreckage.GetOccupiedTiles().Contains((mapX, mapY)) ||
                wreckage.GetOccupiedTiles().Contains((mapX, treeBaseY))
            ) continue;

            switch (_gameLogic.GameMap.GetTileType((mapX, treeBaseY)))
            {
                case TileType.Dirt:
                case TileType.Sand:
                    if (rnd.Next(100) < 95) continue;
                    break;
                case TileType.VegetationSparse:
                    if (rnd.Next(100) < 70) continue;
                    break;
                case TileType.VegetationModerate:
                    if (rnd.Next(100) < 50) continue;
                    break;
                case TileType.VegetationDense:
                    if (rnd.Next(100) < 20) continue;
                    break;
                default:
                    continue;
            }

            _gameLogic.AddEntity(new Tree { MapPosition = (mapX, mapY) });
        }

        // Place colonists
        for (var i = 0; i < CrewSize - 1; i++)
            _gameLogic.AddEntity(new Colonist
            {
                MapPosition = wreckage.GetShipExitTile(),
                ReadableName = Datasets.MaleNames.RandomItem()
            });

        // Place captain
        _gameLogic.AddEntity(new Colonist
        {
            MapPosition = wreckage.GetShipExitTile(),
            ReadableName = $"Captain {_captainName}"
        });
    }
}