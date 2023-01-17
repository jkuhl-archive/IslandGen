namespace IslandGen.Data;

public class GameMap
{
    public readonly int MapSize;
    public readonly TileType[,] TileMap;

    /// <summary>
    ///     Constructor for GameMap
    /// </summary>
    /// <param name="mapSize"> Integer the represents the width and height of the map </param>
    public GameMap(int mapSize)
    {
        MapSize = mapSize;
        TileMap = new TileType[mapSize, mapSize];
    }
}