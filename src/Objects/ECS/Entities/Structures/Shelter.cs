using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Shelter : StructureBase
{
    public const string Description = "Basic structure that a colonist can live in";
    public const string Requirements = "None";
    public static readonly Dictionary<Resource, int> Cost = new() { { Resource.Lumber, 40 } };

    /// <summary>
    ///     Structure that provides basic housing for colonists
    /// </summary>
    public Shelter()
    {
        MiniMapColor = Raylib.BROWN;
        PlaceableOnWater = false;
        ReadableName = "Shelter";
        Size = (2, 2);
        Texture = Assets.Textures["structures/shelter"];

        AddComponent(new Construction { RequiredWork = 5 });
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