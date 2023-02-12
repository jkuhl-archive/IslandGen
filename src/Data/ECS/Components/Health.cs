using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class Health : IComponent
{
    [JsonProperty] public int MaxHealthPoints { get; init; }
    [JsonProperty] public int HealthPoints { get; init; }

    /// <summary>
    ///     Returns info about entity's health
    /// </summary>
    /// <returns> Current and max health points as a string </returns>
    public string GetInfoString()
    {
        return $"{HealthPoints} / {MaxHealthPoints}";
    }
}