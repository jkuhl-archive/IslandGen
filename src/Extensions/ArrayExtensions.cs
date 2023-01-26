using IslandGen.Data.Enum;

namespace IslandGen.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    ///     Checks if the given position is in the array's range
    /// </summary>
    /// <param name="array"> Array we are checking </param>
    /// <param name="x"> Value for the array's first dimension </param>
    /// <param name="y"> Value for the array's second dimension </param>
    /// <returns> True if in range, False if not </returns>
    public static bool InRange(this TileType[,] array, int x, int y)
    {
        if (x < 0)
            return false;

        if (y < 0)
            return false;

        if (x >= array.GetLength(0))
            return false;

        if (y >= array.GetLength(1))
            return false;

        return true;
    }
}