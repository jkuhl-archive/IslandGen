using IslandGen.Data;

namespace IslandGen.Objects.ECS.Entities;

public class Tree : EntityBase
{
    /// <summary>
    ///     Palm tree entity that grows naturally on the island
    /// </summary>
    public Tree()
    {
        ReadableName = "Tree";
        Size = (1, 2);
        Texture = Assets.Textures["tree"];
    }
}