using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen;

public static class MapGeneration
{
    private static readonly Random Rnd = new();
    private static readonly GameMap GameMap = new(800);

    private static readonly Rectangle IslandBaseArea = new(100, 100, 600, 600);
    
    private const int RockDeformPass1MaxTimes = 10;
    private const int RockDeformPass1MaxSize = 120;
    private const int RockDeformPass2MaxTimes = 20;
    private const int RockDeformPass2MaxSize = 50;
    
    private const int SandPadPass1MaxTimes = 50;
    private const int SandPadPass2MaxTimes = 20;
    private const int SandDeformPass1MaxTimes = 15;
    private const int SandDeformPass1MaxSize = 100;
    private const int SandDeformPass2MaxTimes = 1000;
    private const int SandDeformPass2MaxSize = 20;
    private const int SandDeformPass3MaxTimes = 100;
    private const int SandDeformPass3MaxSize = 50;
    
    /// <summary>
    /// Randomly generates a new map and returns it
    /// </summary>
    /// <returns> Newly generated GameMap object </returns>
    public static GameMap GenerateMap()
    {
        // Fill entire map with ocean and then fill island base area with rock
        FillMapSection(Vector2.Zero, new Vector2(GameMap.MapSize), TileType.Ocean);
        FillMapSection(IslandBaseArea.Start(), IslandBaseArea.End(), TileType.Rock);
        
        // Rock deform pass 1 and 2
        DeformMapEdge(IslandBaseArea.Start(), IslandBaseArea.End(), RockDeformPass1MaxTimes, RockDeformPass1MaxSize, TileType.Rock, TileType.Ocean);
        DeformMapEdge(IslandBaseArea.Start(), IslandBaseArea.End(), RockDeformPass2MaxTimes, RockDeformPass2MaxSize, TileType.Rock, TileType.Ocean);
        
        // Rock padding pass 1
        PadMapEdge(TileType.Ocean, TileType.Rock, TileType.Rock);

        // Sand padding pass 1
        PadMapEdge(TileType.Ocean, TileType.Rock, TileType.Sand);
        for (var i = 0; i < Rnd.Next(SandPadPass1MaxTimes); i++)
        {
            PadMapEdge(TileType.Ocean, TileType.Sand, TileType.Sand);
        }

        // Sand deform pass 1 and 2
        DeformMapEdge(IslandBaseArea.Start(), IslandBaseArea.End(), SandDeformPass1MaxTimes, SandDeformPass1MaxSize, TileType.Sand, TileType.Ocean);
        DeformMapEdge(IslandBaseArea.Start(), IslandBaseArea.End(), SandDeformPass2MaxTimes, SandDeformPass2MaxSize, TileType.Sand, TileType.Ocean);
        
        // Sand padding pass 2
        PadMapEdge(TileType.Ocean, TileType.Rock, TileType.Sand);
        for (var i = 0; i < Rnd.Next(SandPadPass2MaxTimes); i++)
        {
            PadMapEdge(TileType.Ocean, TileType.Sand, TileType.Sand);
        }

        // Sand deform pass 3
        DeformMapEdge(IslandBaseArea.Start(), IslandBaseArea.End(), SandDeformPass3MaxTimes, SandDeformPass3MaxSize, TileType.Sand, TileType.Ocean);

        // Sand padding final pass for polish
        PadMapEdge(TileType.Ocean, TileType.Sand, TileType.Sand);

        return GameMap;
    }
    
    /// <summary>
    ///     Deforms edges of the map by replacing random subsections of it
    /// </summary>
    /// <param name="start"> Starting point for the deform operation (top left corner) </param>
    /// <param name="end"> Ending point for the deform operation (bottom right corner) </param>
    /// <param name="deformMaxTimes"> Number of times each side should be deformed </param>
    /// <param name="deformMaxSize"> Maximum size of a randomly selected subsection </param>
    /// <param name="replaceTileType"> TileType that should be replaced if located in a subsection </param>
    /// <param name="fillTileType"> TileType the subsections should be filled with </param>
    private static void DeformMapEdge(Vector2 start, Vector2 end, int deformMaxTimes, int deformMaxSize, TileType replaceTileType, TileType fillTileType)
    {
        for (var i = 0; i < Rnd.Next(deformMaxTimes); i++)
        {
            var deformStartPoints = new List<Vector2>
            {
                start with { X = Rnd.Next(start.X_int(), end.X_int()) },
                start with { Y = Rnd.Next(start.Y_int(), end.Y_int()) },
                end with { X = Rnd.Next(start.X_int(), end.X_int()) },
                end with { Y = Rnd.Next(start.Y_int(), end.Y_int()) }
            };

            foreach (var deformStart in deformStartPoints)
            {
                var deformSize = Rnd.Next(deformMaxSize);

                for (var mapX = deformStart.X_int() - deformSize; mapX < deformStart.X_int() + deformSize; mapX++)
                for (var mapY = deformStart.Y_int() - deformSize; mapY < deformStart.Y_int() + deformSize; mapY++)
                {
                    if (GameMap.TileMap.InRange(mapX, mapY) && GameMap.TileMap[mapX, mapY] == replaceTileType)
                    {
                        GameMap.TileMap[mapX, mapY] = fillTileType;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Fills a section of the map
    /// </summary>
    /// <param name="start"> Starting point for the fill operation (top left corner) </param>
    /// <param name="end"> Ending point for the fill operation (bottom right corner) </param>
    /// <param name="fill"> TileType the section should be filled with </param>
    private static void FillMapSection(Vector2 start, Vector2 end, TileType fill)
    {
        for (var mapX = start.X_int(); mapX < end.X_int(); mapX++)
        for (var mapY = start.Y_int(); mapY < end.Y_int(); mapY++)
            if (GameMap.TileMap.InRange(mapX, mapY))
                GameMap.TileMap[mapX, mapY] = fill;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType1"></param>
    /// <param name="tileType2"></param>
    /// <param name="fill"></param>
    private static void PadMapEdge(TileType tileType1, TileType tileType2, TileType fill)
    {
        var tilesPendingUpdate = new List<(int, int)>();

        // Find tiles that exist at the edge of tileType1 and tileType2 and mark them to be updated
        for (var mapX = 0; mapX < GameMap.MapSize; mapX++)
        {
            for (var mapY = 0; mapY < GameMap.MapSize; mapY++)
            {
                var currentTile = GameMap.TileMap[mapX, mapY];
                var adjacentTiles = new List<(int, int)>
                {
                    (mapX, mapY + 1),
                    (mapX, mapY - 1),
                    (mapX + 1, mapY),
                    (mapX - 1, mapY)
                };

                foreach (var adjacentTile in adjacentTiles)
                {
                    if (GameMap.TileMap.InRange(adjacentTile.Item1, adjacentTile.Item2) &&
                        currentTile == tileType1 &&
                        GameMap.TileMap[adjacentTile.Item1, adjacentTile.Item2] == tileType2)
                    {
                        tilesPendingUpdate.Add((mapX, mapY));
                        break;
                    }
                }
            }
        }

        // Update all marked tiles
        foreach (var tileCoordinates in tilesPendingUpdate)
        {
            GameMap.TileMap[tileCoordinates.Item1, tileCoordinates.Item2] = fill;
        }
    }
}