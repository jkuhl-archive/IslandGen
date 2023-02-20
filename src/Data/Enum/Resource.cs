namespace IslandGen.Data.Enum;

/// <summary>
///     List of resources that can be generated and used by the colony
/// </summary>
public enum Resource
{
    CookedFish,
    DrinkingWater,
    Lumber,
    RawFish,
    Stone,
    WildGrains
}

public static class ResourceExtensions
{
    /// <summary>
    ///     Gets a readable name for the given Resource
    /// </summary>
    /// <param name="resource"> Resource we are getting the name of </param>
    /// <returns> Readable name as a string </returns>
    /// <exception cref="ArgumentOutOfRangeException"> If unknown value is passed in </exception>
    public static string GetResourceName(this Resource resource)
    {
        return resource switch
        {
            Resource.CookedFish => "Cooked Fish",
            Resource.DrinkingWater => "Drinking Water",
            Resource.Lumber => "Lumber",
            Resource.RawFish => "Raw Fish",
            Resource.Stone => "Stone",
            Resource.WildGrains => "Wild Grains",
            _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null)
        };
    }
}