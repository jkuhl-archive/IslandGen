using Raylib_CsLo;

namespace IslandGen.Data;

/// <summary>
///     Contents of a single tile on the map
/// </summary>
public enum TileType
{
    Debug,
    Ocean,
    Dirt,
    Rock,
    Lake,
    Sand,
    River
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
            TileType.Dirt => Raylib.BROWN,
            TileType.Rock => Raylib.DARKGRAY,
            TileType.Lake => Raylib.DARKBLUE,
            TileType.Sand => Raylib.BEIGE,
            TileType.River => Raylib.DARKBLUE,
            _ => Raylib.PURPLE
        };
    }
}