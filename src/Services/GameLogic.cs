using IslandGen.Data.ECS;
using IslandGen.Data.ECS.Entities;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    private const int StartYear = 1600;
    private const int StartMonth = 1;
    private const int StartDay = 1;

    public readonly List<Colonist> Colonists;
    [JsonIgnore] public readonly DateTime StartDateTime = new(StartYear, StartMonth, StartDay);
    public readonly List<Structure> Structures;

    private Structure? _mouseStructure;
    private float _updateTimer;

    /// <summary>
    ///     Service that manages the game's logic
    /// </summary>
    public GameLogic()
    {
        Colonists = new List<Colonist>();
        Structures = new List<Structure>();
        CurrentDateTime = StartDateTime;
        GameSpeed = GameSpeed.Normal;
    }

    /// <summary>
    ///     Constructor for loading a saved GameLogic
    /// </summary>
    /// <param name="colonists"> List of colonists </param>
    /// <param name="structures"> List of structures </param>
    /// <param name="currentDateTime"> Current DateTime in game </param>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    [JsonConstructor]
    private GameLogic(List<Colonist> colonists, List<Structure> structures, DateTime currentDateTime,
        GameSpeed gameSpeed)
    {
        Colonists = colonists;
        Structures = structures;
        CurrentDateTime = currentDateTime;
        GameSpeed = gameSpeed;
    }

    public DateTime CurrentDateTime { get; private set; }
    public GameSpeed GameSpeed { get; private set; }

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var colonist in Colonists) colonist.Draw();
        foreach (var structure in Structures) structure.Draw();

        _mouseStructure?.Draw();
    }

    public void Update()
    {
        // Update objects that rely on in game time passage
        _updateTimer += Raylib.GetFrameTime();
        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            _updateTimer = 0;
            CurrentDateTime = CurrentDateTime.AddHours(1);
            ServiceManager.GetService<GameMap>().Update();
            foreach (var colonist in Colonists) colonist.Update();
        }

        // Update mouse structure position to match mouse cursor position 
        if (_mouseStructure != null)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            _mouseStructure.MapPosition = gameMap.GetMapMouseTile();
        }
    }

    /// <summary>
    ///     Toggles current GameSpeed to the next value
    /// </summary>
    public void ChangeSpeed()
    {
        GameSpeed = GameSpeed.GetNext();
    }

    /// <summary>
    ///     Attempts to place the selected structure on the map at the mouse cursors position
    /// </summary>
    public void PlaceMouseStructure()
    {
        if (_mouseStructure == null) return;
        var occupiedTiles = _mouseStructure.GetStructureOccupiedTiles();

        // Check if structure is on water
        if (!_mouseStructure.PlaceableOnWater)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            foreach (var tile in occupiedTiles)
                if (gameMap.TileMap[tile.Item1, tile.Item2].IsWater())
                    return;
        }

        // Check if the structure will overlap with any existing structures
        if (Structures.Any(structure => occupiedTiles.Intersect(structure.GetStructureOccupiedTiles()).Any())) return;

        // If all checks pass, add the structure to the main list and remove it from the mouse
        Structures.Add(_mouseStructure);
        _mouseStructure = null;
    }

    /// <summary>
    ///     Sets a structure to the mouse cursor
    /// </summary>
    /// <param name="structure"> Structure that is being set to the mouse cursor </param>
    public void SetMouseStructure(Structure structure)
    {
        _mouseStructure = structure;
    }
}