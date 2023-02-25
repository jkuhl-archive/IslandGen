using IslandGen.Data;
using IslandGen.Objects.ECS.Components;
using IslandGen.Objects.ECS.Routines;

namespace IslandGen.Objects.ECS.Entities.Creatures;

public class Colonist : EntityBase
{
    /// <summary>
    ///     Colonist entity
    /// </summary>
    public Colonist()
    {
        MiniMapColor = Colors.MiniMapColonist;
        Size = (1, 1);
        Texture = Assets.Textures["creatures/colonist"];

        AddComponent(new Health { MaxHealthPoints = 100, HealthPoints = 100 });
        AddComponent(new Inventory { InventorySize = 10 });
        AddComponent(new MovementSpeed { Speed = 1 });
        AddRoutine(new Build());
        AddRoutine(new Wander());
    }
}