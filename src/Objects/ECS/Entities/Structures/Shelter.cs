using IslandGen.Data;
using IslandGen.Objects.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Shelter : StructureBase
{
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
}