using IslandGen.Data.ECS;
using IslandGen.Data.ECS.Entities;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    public readonly List<Colonist> Colonists;
    public readonly List<Structure> Structures;
    private Structure? _mouseStructure;
    private float _updateTimer;
    public GameSpeed GameSpeed;

    /// <summary>
    ///     Service that manages the game's logic
    /// </summary>
    public GameLogic()
    {
        Colonists = new List<Colonist>();
        Structures = new List<Structure>();
        GameSpeed = GameSpeed.Normal;
    }

    /// <summary>
    ///     Constructor for loading a saved GameLogic
    /// </summary>
    /// <param name="colonists"> List of colonists </param>
    /// <param name="structures"> List of structures </param>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    [JsonConstructor]
    private GameLogic(List<Colonist> colonists, List<Structure> structures, GameSpeed gameSpeed)
    {
        Colonists = colonists;
        Structures = structures;
        GameSpeed = gameSpeed;
    }

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var colonist in Colonists) colonist.Draw();
        foreach (var structure in Structures) structure.Draw();

        _mouseStructure?.Draw();
    }

    public void Update()
    {
        _updateTimer += Raylib.GetFrameTime();

        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            foreach (var colonist in Colonists) colonist.Update();
            _updateTimer = 0;
        }

        if (_mouseStructure != null)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            _mouseStructure.MapPosition = gameMap.GetMapMouseTile();
        }
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