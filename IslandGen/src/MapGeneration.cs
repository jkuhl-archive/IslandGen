using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;

namespace IslandGen;

public static class MapGeneration
{
    private const int RockDeformPass1MaxTimes = 10;
    private const int RockDeformPass2MaxTimes = 20;
    private const int SandDeformPass1MaxTimes = 15;
    private const int SandDeformPass2MaxTimes = 1000;
    private const int SandDeformPass3MaxTimes = 100;
    
    private const float RockDeformPass1MaxSizeMultiplier = 0.15f;
    private const float RockDeformPass2MaxSizeMultiplier = 0.06f;
    private const float SandPadPass1MaxTimesMultiplier = 0.06f;
    private const float SandPadPass2MaxTimesMultiplier = 0.02f;
    
    private const float SandDeformPass1MaxSizeMultiplier = 0.10f;
    private const float SandDeformPass2MaxSizeMultiplier = 0.02f;
    private const float SandDeformPass3MaxSizeMultiplier = 0.06f;

    private static readonly Random Rnd = new();
    
    /// <summary>
    ///     Randomly generates a new map and returns it
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to generate </param>
    public static void GenerateMap(GameMap gameMap)
    {
        // Fill island base area with rock
        gameMap.FillMapSection(gameMap.BaseIslandArea.Start(), gameMap.BaseIslandArea.End(), TileType.Rock);

        // Rock deform pass 1 and 2
        DeformMapEdge(gameMap, RockDeformPass1MaxTimes, (int)(gameMap.MapSize * RockDeformPass1MaxSizeMultiplier), TileType.Rock, TileType.Ocean);
        DeformMapEdge(gameMap, RockDeformPass2MaxTimes, (int)(gameMap.MapSize * RockDeformPass2MaxSizeMultiplier), TileType.Rock, TileType.Ocean);

        // Rock padding pass 1
        PadMapEdge(gameMap, TileType.Ocean, TileType.Rock, TileType.Rock);

        // Sand padding pass 1
        PadMapEdge(gameMap, TileType.Ocean, TileType.Rock, TileType.Sand);
        for (var i = 0; i < Rnd.Next((int)(gameMap.MapSize * SandPadPass1MaxTimesMultiplier)); i++)
            PadMapEdge(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);

        // Sand deform pass 1 and 2
        DeformMapEdge(gameMap, SandDeformPass1MaxTimes, (int)(gameMap.MapSize * SandDeformPass1MaxSizeMultiplier), TileType.Sand, TileType.Ocean);
        DeformMapEdge(gameMap, SandDeformPass2MaxTimes, (int)(gameMap.MapSize * SandDeformPass2MaxSizeMultiplier), TileType.Sand, TileType.Ocean);

        // Sand padding pass 2
        PadMapEdge(gameMap, TileType.Ocean, TileType.Rock, TileType.Sand);
        for (var i = 0; i < Rnd.Next((int)(gameMap.MapSize * SandPadPass2MaxTimesMultiplier)); i++)
            PadMapEdge(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);

        // Sand deform pass 3
        DeformMapEdge(gameMap, SandDeformPass3MaxTimes, (int)(gameMap.MapSize * SandDeformPass3MaxSizeMultiplier), TileType.Sand, TileType.Ocean);

        // Sand padding final pass for polish
        PadMapEdge(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);
    }

    /// <summary>
    ///     Deforms edges of the map by replacing random subsections of it
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to deform </param>
    /// <param name="deformMaxTimes"> Number of times each side should be deformed </param>
    /// <param name="deformMaxSize"> Maximum size of a randomly selected subsection </param>
    /// <param name="replaceTileType"> TileType that should be replaced if located in a subsection </param>
    /// <param name="fillTileType"> TileType the subsections should be filled with </param>
    private static void DeformMapEdge(GameMap gameMap, int deformMaxTimes, int deformMaxSize, TileType replaceTileType, TileType fillTileType)
    {
        var start = gameMap.BaseIslandArea.Start();
        var end = gameMap.BaseIslandArea.End();
        
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
                    if (gameMap.TileMap.InRange(mapX, mapY) && gameMap.TileMap[mapX, mapY] == replaceTileType)
                        gameMap.TileMap[mapX, mapY] = fillTileType;
            }
        }
    }

    /// <summary>
    ///     Pads the map edge by replacing tiles where one TileType touches another
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to pad </param>
    /// <param name="tileType1"> TileType that we are checking for as the transition, replaced if valid </param>
    /// <param name="tileType2"> TileType that we are checking for as the edge </param>
    /// <param name="fill"> TileType that valid transition tiles should be replaced with </param>
    private static void PadMapEdge(GameMap gameMap, TileType tileType1, TileType tileType2, TileType fill)
    {
        var tilesPendingUpdate = new List<(int, int)>();

        // Find tiles that exist at the edge of tileType1 and tileType2 and mark them to be updated
        for (var mapX = 0; mapX < gameMap.MapSize; mapX++)
        for (var mapY = 0; mapY < gameMap.MapSize; mapY++)
        {
            var currentTile = gameMap.TileMap[mapX, mapY];
            var adjacentTiles = new List<(int, int)>
            {
                (mapX, mapY + 1),
                (mapX, mapY - 1),
                (mapX + 1, mapY),
                (mapX - 1, mapY)
            };

            foreach (var adjacentTile in adjacentTiles)
                if (gameMap.TileMap.InRange(adjacentTile.Item1, adjacentTile.Item2) &&
                    currentTile == tileType1 &&
                    gameMap.TileMap[adjacentTile.Item1, adjacentTile.Item2] == tileType2)
                {
                    tilesPendingUpdate.Add((mapX, mapY));
                    break;
                }
        }

        // Update all marked tiles
        foreach (var tileCoordinates in tilesPendingUpdate)
            gameMap.TileMap[tileCoordinates.Item1, tileCoordinates.Item2] = fill;
    }
}