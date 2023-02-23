using IslandGen.Data;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Structures;

public class Well : StructureBase
{
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
    }
}