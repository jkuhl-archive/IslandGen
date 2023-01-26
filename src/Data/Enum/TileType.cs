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
    Sand
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
            TileType.Debug => Raylib.PINK,
            TileType.Dirt => Raylib.BROWN,
            TileType.Lake => Raylib.DARKBLUE,
            TileType.Ocean => Raylib.BLUE,
            TileType.River => Raylib.DARKBLUE,
            TileType.Rock => Raylib.GRAY,
            TileType.Sand => Raylib.BEIGE,
            _ => Raylib.PURPLE
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
            TileType.Debug => "debug",
            TileType.Dirt => "dirt",
            TileType.Lake => "lake",
            TileType.Ocean => "ocean",
            TileType.River => "river",
            TileType.Rock => "rock",
            TileType.Sand => "sand",
            _ => "debug"
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
}