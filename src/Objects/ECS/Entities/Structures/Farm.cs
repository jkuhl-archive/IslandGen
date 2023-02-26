using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Farm : StructureBase
{
    public const string Description = "Plot for colonists to grow crops";
    public const string Requirements = "Must be placed on soil";
    public static readonly Dictionary<Resource, int> Cost = new() { { Resource.Lumber, 30 } };

    /// <summary>
    ///     Structure generates crops
    /// </summary>
    public Farm()
    {
        MiniMapColor = Raylib.BROWN;
        PlaceableOnWater = false;
        ReadableName = "Farm";
        Size = (4, 4);
        Texture = Assets.Textures["structures/farm"];

        AddComponent(new Construction { RequiredWork = 2 });
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
    ///     Checks if the current position of the farm is valid
    /// </summary>
    /// <returns> True if farm can be placed here, false if not </returns>
    public override bool ValidPlacement()
    {
        if (!base.ValidPlacement()) return false;

        var gameLogic = ServiceManager.GetService<GameLogic>();
        return GetOccupiedTiles().All(tile => gameLogic.GameMap.GetTileType(tile).IsGrowable());
    }
}