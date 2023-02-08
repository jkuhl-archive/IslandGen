using IslandGen.Services;

namespace IslandGen.Data.ECS.Entities.Structures;

public class Shelter : Structure
{
    /// <summary>
    ///     Structure that provides basic housing for colonists
    /// </summary>
    public Shelter()
    {
        PlaceableOnWater = false;
        ReadableName = "Shelter";
        Size = (2, 2);
        Texture = ServiceManager.GetService<TextureManager>().Textures["shelter"];
    }
}