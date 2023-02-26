using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class FishingSpot : StructureBase
{
    public const string Description = "Allows colonists to fish in nearby water";
    public const string Requirements = "Must be placed near water";
    public static readonly Dictionary<Resource, int> Cost = new() { { Resource.Lumber, 10 }, { Resource.Stone, 10 } };

    /// <summary>
    ///     Structure generates raw fish
    /// </summary>
    public FishingSpot()
    {
        MiniMapColor = Raylib.BROWN;
        PlaceableOnWater = false;
        ReadableName = "Fishing Spot";
        Size = (1, 1);
        Texture = Assets.Textures["structures/fishing_spot"];

        AddComponent(new Construction { RequiredWork = 1 });
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
    ///     Checks if the current position of the fishing spot is valid
    /// </summary>
    /// <returns> True if fishing spot can be placed here, false if not </returns>
    public override bool ValidPlacement()
    {
        if (!base.ValidPlacement()) return false;

        var gameLogic = ServiceManager.GetService<GameLogic>();
        var occupiedTile = GetOccupiedTiles()[0];
        var adjacentTiles = new List<(int, int)>
        {
            (occupiedTile.Item1, occupiedTile.Item2 + 1),
            (occupiedTile.Item1, occupiedTile.Item2 - 1),
            (occupiedTile.Item1 + 1, occupiedTile.Item2),
            (occupiedTile.Item1 - 1, occupiedTile.Item2)
        };
        return adjacentTiles.Any(tile => gameLogic.GameMap.GetTileType(tile).IsWater());
    }
}