using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class MovementSpeed : IComponent
{
    [JsonProperty] public readonly int Speed;

    /// <summary>
    ///     Component that manages the entity's movement speed
    /// </summary>
    /// <param name="speed"> Movement speed </param>
    public MovementSpeed(int speed = 1)
    {
        Speed = speed;
    }

    /// <summary>
    ///     Returns info about entity's movement speed
    /// </summary>
    /// <returns> Movement speed as a string </returns>
    public string GetInfoString()
    {
        return $"{Speed}";
    }
}