using IslandGen.Data;
using IslandGen.Data.Enum;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class LumberYard : StructureBase
{
    public const string Description = "Allows colonists to chop down trees and create lumber";
    public const string Requirements = "None";

    public static readonly Dictionary<Resource, int>
        Cost = new() { { Resource.Lumber, 10 }, { Resource.TreeTrunk, 2 } };

    /// <summary>
    ///     Structure generates and stores lumber
    /// </summary>
    public LumberYard()
    {
        ConstructionTotalWork = 5;
        MiniMapColor = Raylib.BROWN;
        PlaceableOnWater = false;
        ReadableName = "Lumber Yard";
        Size = (3, 3);
        Texture = Assets.Textures["structures/lumber_yard"];
    }

    /// <summary>
    ///     Gets the cost of this structure
    /// </summary>
    /// <returns> Cost of this structure as a dictionary of resources and amounts </returns>
    public override Dictionary<Resource, int> GetCost()
    {
        return Cost;
    }
}