using IslandGen.Services;

namespace IslandGen.Data.ECS.Entities;

public class Tree : EntityBase
{
    /// <summary>
    ///     Palm tree entity that grows naturally on the island
    /// </summary>
    public Tree()
    {
        ReadableName = "Tree";
        Size = (1, 2);
        Texture = ServiceManager.GetService<TextureManager>().Textures["tree"];
    }
}