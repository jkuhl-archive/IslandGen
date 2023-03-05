using IslandGen.Data.Enum;
using IslandGen.Services;

namespace IslandGen.Utils;

public static class CostUtils
{
    /// <summary>
    ///     Determines if the colony has enough resources to afford the given cost
    /// </summary>
    /// <param name="cost"> Cost that we are checking </param>
    /// <returns> True if can afford, false if not </returns>
    public static bool CanAfford(Dictionary<Resource, int> cost)
    {
        var resources = ServiceManager.GetService<GameLogic>().GetResourceCounts();
        return cost.All(i => resources.ContainsKey(i.Key) && resources[i.Key] >= i.Value);
    }

    /// <summary>
    ///     Generates a nicely formatted string that summarizes cost
    /// </summary>
    /// <param name="cost"> Dictionary of resources and ints that represents a cost </param>
    /// <returns> String containing cost info </returns>
    public static string GetCostString(Dictionary<Resource, int> cost)
    {
        return string.Join(", ", cost.Select(item => $"{item.Key.GetResourceName()}: {item.Value}").ToList());
    }
}