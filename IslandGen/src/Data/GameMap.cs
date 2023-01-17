using System.Numerics;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Data;

public class GameMap
{
    public readonly int MapSize;
    public readonly Rectangle BaseIslandArea;
    public readonly TileType[,] TileMap;

    /// <summary>
    ///     Constructor for GameMap
    /// </summary>
    /// <param name="mapSize"> Integer that represents the width and height of the map </param>
    public GameMap(int mapSize)
    {
        MapSize = mapSize;
        
        var buffer = MapSize / 10;
        var baseIslandSize = mapSize - buffer * 2;
        BaseIslandArea = new Rectangle(buffer, buffer, baseIslandSize, baseIslandSize);
        
        TileMap = new TileType[mapSize, mapSize];
        FillMapSection(Vector2.Zero, new Vector2(MapSize), TileType.Ocean);
    }
    
    /// <summary>
    ///     Fills a section of the map
    /// </summary>
    /// <param name="start"> Starting point for the fill operation (top left corner) </param>
    /// <param name="end"> Ending point for the fill operation (bottom right corner) </param>
    /// <param name="fill"> TileType the section should be filled with </param>
    public void FillMapSection(Vector2 start, Vector2 end, TileType fill)
    {
        for (var mapX = start.X_int(); mapX < end.X_int(); mapX++)
        for (var mapY = start.Y_int(); mapY < end.Y_int(); mapY++)
            if (TileMap.InRange(mapX, mapY))
                TileMap[mapX, mapY] = fill;
    }
}