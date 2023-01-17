using System.Numerics;
using Raylib_CsLo;

namespace IslandGen.Data;

public class GameMap
{
    public readonly int MapSize;
    public readonly TileType[,] TileMap;

    public GameMap(int mapSize)
    {
        MapSize = mapSize;
        TileMap = new TileType[mapSize, mapSize];
    }
}