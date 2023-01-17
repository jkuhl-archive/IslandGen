using Raylib_CsLo;

namespace IslandGen.Data;

/// <summary>
///     Tracks possible tile contents for the map
/// </summary>
public enum TileType
{
    Debug,
    Ocean,
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
            TileType.Ocean => Raylib.BLUE,
            TileType.Rock => Raylib.DARKGRAY,
            TileType.Sand => Raylib.BEIGE,
            _ => Raylib.PURPLE
        };
    }
}