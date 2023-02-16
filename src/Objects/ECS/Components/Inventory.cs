using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Components;

public class Inventory : IComponent
{
    [JsonProperty] public int InventorySize { get; init; }
    [JsonProperty] public List<EntityBase> InventoryContents { get; } = new();

    /// <summary>
    ///     Returns info about the entity's inventory
    /// </summary>
    /// <returns> Currently used and max inventory slots as a string </returns>
    public string GetInfoString()
    {
        return $"{InventoryContents.Count} / {InventorySize}";
    }
}