using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Well : StructureBase
{
    public const string Description = "Allows colonists to retrieve and store fresh water";
    public const string Requirements = "None";
    public static readonly Dictionary<Resource, int> Cost = new() { { Resource.Lumber, 5 }, { Resource.Stone, 20 } };

    /// <summary>
    ///     Structure that generates drinking water
    /// </summary>
    public Well()
    {
        MiniMapColor = Raylib.DARKGRAY;
        PlaceableOnWater = false;
        ReadableName = "Well";
        Size = (1, 1);
        Texture = Assets.Textures["structures/well"];

        AddComponent(new Construction { RequiredWork = 10 });
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