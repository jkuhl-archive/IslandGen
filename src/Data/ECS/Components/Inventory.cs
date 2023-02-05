namespace IslandGen.Data.ECS.Components;

public class Inventory : IComponent
{
    /// <summary>
    ///     Component that manages entity's inventory
    /// </summary>
    /// <param name="inventorySize"> Amount of space in the entity's inventory </param>
    public Inventory(int inventorySize = 10)
    {
        InventorySize = inventorySize;
        InventoryContents = new List<EntityBase>();
    }

    public int InventorySize { get; }
    public List<EntityBase> InventoryContents { get; }
}