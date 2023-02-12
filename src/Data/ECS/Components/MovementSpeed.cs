using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class MovementSpeed : IComponent
{
    [JsonProperty] public int Speed { get; init; }

    /// <summary>
    ///     Returns info about entity's movement speed
    /// </summary>
    /// <returns> Movement speed as a string </returns>
    public string GetInfoString()
    {
        return $"{Speed}";
    }
}