using IslandGen.Data;

namespace IslandGen.Extensions;

public static class ArrayExtensions
{
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