namespace IslandGen.Data.ECS.Components;

public class Inventory : IComponent
{
    public Inventory(int inventorySize)
    {
        InventorySize = inventorySize;
        InventoryContents = new List<IEntity>();
    }

    public int InventorySize { get; }
    public List<IEntity> InventoryContents { get; }
}