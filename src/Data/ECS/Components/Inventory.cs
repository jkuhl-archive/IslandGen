using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class Inventory : IComponent
{
    [JsonProperty] public readonly int InventorySize;

    /// <summary>
    ///     Component that manages entity's inventory
    /// </summary>
    /// <param name="inventorySize"> Amount of space in the entity's inventory </param>
    public Inventory(int inventorySize = 10)
    {
        InventorySize = inventorySize;
        InventoryContents = new List<EntityBase>();
    }

    [JsonProperty] public List<EntityBase> InventoryContents { get; }

    /// <summary>
    ///     Returns info about the entity's inventory
    /// </summary>
    /// <returns> Currently used and max inventory slots as a string </returns>
    public string GetInfoString()
    {
        return $"{InventoryContents.Count} / {InventorySize}";
    }
}