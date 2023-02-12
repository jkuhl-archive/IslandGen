using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class Health : IComponent
{
    [JsonProperty] public readonly int MaxHealthPoints;

    /// <summary>
    ///     Component that manages the entity's health
    /// </summary>
    /// <param name="maxHealthPoints"> Maximum number of health points for the entity </param>
    public Health(int maxHealthPoints = 100)
    {
        MaxHealthPoints = maxHealthPoints;
        HealthPoints = MaxHealthPoints;
    }

    [JsonProperty] public int HealthPoints { get; }

    /// <summary>
    ///     Returns info about entity's health
    /// </summary>
    /// <returns> Current and max health points as a string </returns>
    public string GetInfoString()
    {
        return $"{HealthPoints} / {MaxHealthPoints}";
    }
}