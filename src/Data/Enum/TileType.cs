using Raylib_CsLo;

namespace IslandGen.Data.Enum;

/// <summary>
///     Contents of a single tile on the map
/// </summary>
public enum TileType
{
    Debug,
    Dirt,
    Lake,
    Ocean,
    River,
    Rock,
    Sand,
    VegetationDense,
    VegetationModerate,
    VegetationSparse
}

public static class TileTypeExtensions
{
    /// <summary>
    ///     Returns the designated Color for the given TileType
    /// </summary>
    /// <param name="tileType"> TileType that we want to get the Color for </param>
    /// <returns> Color for the given TileType </returns>
    public static Color GetTileColor(this TileType tileType)
    {
        return tileType switch
        {
            TileType.Debug => Colors.TileDebug,
            TileType.Dirt => Colors.TileDirt,
            TileType.Lake => Colors.TileLake,
            TileType.Ocean => Colors.TileOcean,
            TileType.River => Colors.TileRiver,
            TileType.Rock => Colors.TileRock,
            TileType.Sand => Colors.TileSand,
            TileType.VegetationDense => Colors.TileVegetationDense,
            TileType.VegetationModerate => Colors.TileVegetationModerate,
            TileType.VegetationSparse => Colors.TileVegetationSparse,
            _ => Raylib.WHITE
        };
    }

    /// <summary>
    ///     Returns the name of the texture associated with TileType
    /// </summary>
    /// <param name="tileType"> TileType that we want to get the texture name for </param>
    /// <returns> String containing the texture name </returns>
    public static string GetTileTextureName(this TileType tileType)
    {
        return tileType switch
        {
            TileType.Debug => "tiles/debug",
            TileType.Dirt => "tiles/dirt",
            TileType.Lake => "tiles/lake",
            TileType.Ocean => "tiles/ocean",
            TileType.River => "tiles/river",
            TileType.Rock => "tiles/rock",
            TileType.Sand => "tiles/sand",
            TileType.VegetationDense => "tiles/vegetation_3",
            TileType.VegetationModerate => "tiles/vegetation_2",
            TileType.VegetationSparse => "tiles/vegetation_1",
            _ => "tiles/debug"
        };
    }

    /// <summary>
    ///     Returns true if the tile should be animated
    /// </summary>
    /// <param name="tileType"> TileType that we are checking </param>
    /// <returns> True if animated, false if not </returns>
    public static bool IsAnimated(this TileType tileType)
    {
        return tileType switch
        {
            TileType.Lake => true,
            TileType.Ocean => true,
            TileType.River => true,
            _ => false
        };
    }

    /// <summary>
    ///     Returns true if the tile is water
    /// </summary>
    /// <param name="tileType"> TileType that we are checking </param>
    /// <returns> True if water, false if not </returns>
    public static bool IsGrowable(this TileType tileType)
    {
        return tileType switch
        {
            TileType.Dirt => true,
            TileType.VegetationDense => true,
            TileType.VegetationModerate => true,
            TileType.VegetationSparse => true,
            _ => false
        };
    }

    /// <summary>
    ///     Returns true if the tile is water
    /// </summary>
    /// <param name="tileType"> TileType that we are checking </param>
    /// <returns> True if water, false if not </returns>
    public static bool IsWater(this TileType tileType)
    {
        return tileType switch
        {
            TileType.Lake => true,
            TileType.Ocean => true,
            TileType.River => true,
            _ => false
        };
    }
}