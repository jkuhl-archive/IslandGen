using IslandGen.Data.Enum;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class StructureBase : EntityBase
{
    [JsonIgnore] public bool PlaceableOnWater { get; protected init; }

    /// <summary>
    ///     Checks if the current position of the structure is valid
    /// </summary>
    /// <returns> True if structure can be placed here, false if not </returns>
    public virtual bool ValidPlacement()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var occupiedTiles = GetOccupiedTiles();

        // Check if structure is on water
        if (!PlaceableOnWater)
            if (occupiedTiles.Any(tile => gameLogic.GameMap.GetTileType(tile).IsWater()))
                return false;

        // Check if the structure will overlap with any existing structures
        foreach (var structure in gameLogic.GetEntityBaseTypeList<StructureBase>())
            if (structure.GetOccupiedTiles().Intersect(occupiedTiles).Any())
                return false;

        return true;
    }
}