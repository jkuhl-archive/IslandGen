using IslandGen.Data;
using IslandGen.Objects.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class LumberYard : StructureBase
{
    /// <summary>
    ///     Structure generates and stores lumber
    /// </summary>
    public LumberYard()
    {
        MiniMapColor = Raylib.BROWN;
        PlaceableOnWater = false;
        ReadableName = "Lumber Yard";
        Size = (3, 3);
        Texture = Assets.Textures["structures/lumber_yard"];

        AddComponent(new Construction { RequiredWork = 4 });
    }
}