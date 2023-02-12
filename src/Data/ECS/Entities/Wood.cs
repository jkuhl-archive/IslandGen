using IslandGen.Services;

namespace IslandGen.Data.ECS.Entities;

public class Wood : EntityBase
{
    /// <summary>
    ///     It's wood
    /// </summary>
    public Wood()
    {
        ReadableName = "Wood";
        Size = (1, 1);
        Texture = ServiceManager.GetService<TextureManager>().Textures["wood"];
    }
}