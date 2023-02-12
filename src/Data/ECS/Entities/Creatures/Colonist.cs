using IslandGen.Data.ECS.Components;
using IslandGen.Services;

namespace IslandGen.Data.ECS.Entities.Creatures;

public class Colonist : EntityBase
{
    /// <summary>
    ///     Colonist entity
    /// </summary>
    public Colonist()
    {
        Size = (1, 1);
        Texture = ServiceManager.GetService<TextureManager>().Textures["colonist"];

        AddComponent(new Health());
        AddComponent(new Inventory());
        AddComponent(new MovementSpeed());
        AddComponent(new Wander());
    }
}