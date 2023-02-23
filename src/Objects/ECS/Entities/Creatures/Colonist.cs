using IslandGen.Data;
using IslandGen.Objects.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities.Creatures;

public class Colonist : EntityBase
{
    /// <summary>
    ///     Colonist entity
    /// </summary>
    public Colonist()
    {
        MiniMapColor = Raylib.YELLOW;
        Size = (1, 1);
        Texture = Assets.Textures["creatures/colonist"];

        AddComponent(new Health { MaxHealthPoints = 100, HealthPoints = 100 });
        AddComponent(new Inventory { InventorySize = 10 });
        AddComponent(new MovementSpeed { Speed = 1 });
        AddComponent(new Wander());
    }
}