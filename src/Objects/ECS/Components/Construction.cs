using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Components;

public class Construction : IComponent
{
    [JsonProperty] public int Progress { get; private set; }
    [JsonProperty] public int RequiredWork { get; init; }
    [JsonProperty] public bool Started { get; set; }
    [JsonProperty] public bool Complete { get; private set; }

    public void Update(EntityBase entity)
    {
        if (Started) Progress++;
        if (Progress >= RequiredWork) Complete = true;
    }

    /// <summary>
    ///     Returns info about entity's construction status
    /// </summary>
    /// <returns> Construction status and duration as a string </returns>
    public string GetInfoString()
    {
        return Complete ? "Complete" : $"{Progress}/{RequiredWork}";
    }

    /// <summary>
    ///     Gets construction progress as a float from 0.0 - 1.0
    /// </summary>
    /// <returns> Construction progress as a float </returns>
    public float GetProgressPercentage()
    {
        return (float)Progress / RequiredWork;
    }
}