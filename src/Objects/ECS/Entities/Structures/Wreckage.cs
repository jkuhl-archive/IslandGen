using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Wreckage : StructureBase
{
    private static readonly Dictionary<Resource, int> Cost = new() { { Resource.Lumber, 200 }, { Resource.Stone, 25 } };

    /// <summary>
    ///     Ship wreckage structure, colonists can "work" the wreckage to savage materials
    /// </summary>
    public Wreckage()
    {
        ConstructionProgress = 200;
        ConstructionTotalWork = ConstructionProgress;
        MiniMapColor = Raylib.DARKBROWN;
        PlaceableOnWater = true;
        ReadableName = "Ship Wreckage";
        Size = (5, 4);
        Texture = Assets.Textures["structures/wreckage"];
    }

    /// <summary>
    ///     Gets the cost of this structure
    /// </summary>
    /// <returns> Cost of this structure as a dictionary of resources and amounts </returns>
    public override Dictionary<Resource, int> GetCost()
    {
        return Cost;
    }

    /// <summary>
    ///     Gets the tile on the wreckage that colonists exit from
    /// </summary>
    /// <returns> Position on the tile map colonists exit from </returns>
    public (int, int) GetShipExitTile()
    {
        return (MapPosition.Item1 + 3, MapPosition.Item2 + 2);
    }

    /// <summary>
    ///     Checks if the current position of the wreckage is valid
    /// </summary>
    /// <returns> True if wreckage can be placed here, false if not </returns>
    public override bool ValidPlacement()
    {
        if (!base.ValidPlacement()) return false;

        var gameLogic = ServiceManager.GetService<GameLogic>();
        var landTileCounter = 0;
        var landThreshold = Size.Item1 * Size.Item2 / 3;
        var oceanTileCounter = 0;
        var oceanThreshold = Size.Item1 * Size.Item2 / 2;

        foreach (var tile in GetOccupiedTiles())
            if (gameLogic.GameMap.GetTileType(tile) == TileType.Ocean)
                oceanTileCounter++;
            else
                landTileCounter++;

        return oceanTileCounter >= oceanThreshold && landTileCounter >= landThreshold;
    }
}