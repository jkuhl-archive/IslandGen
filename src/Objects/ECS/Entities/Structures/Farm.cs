using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Farm : StructureBase
{
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