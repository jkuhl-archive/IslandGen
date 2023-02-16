using IslandGen.Data;

namespace IslandGen.Objects.ECS.Entities;

public class Wood : EntityBase
{
    /// <summary>
    ///     It's wood
    /// </summary>
    public Wood()
    {
        ReadableName = "Wood";
        Size = (1, 1);
        Texture = Assets.Textures["wood"];
    }
}