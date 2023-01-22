using IslandGen.Data.ECS;

namespace IslandGen.Services;

public class EntityManager
{
    public readonly List<IEntity> Entities;

    /// <summary>
    ///     Constructor for EntityManager
    /// </summary>
    public EntityManager()
    {
        Entities = new List<IEntity>();
    }

    public void Draw()
    {
        foreach (var entity in Entities) entity.Draw();
    }

    public void Update()
    {
        foreach (var entity in Entities) entity.Update();
    }
}