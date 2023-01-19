using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;

namespace IslandGen;

public static class MapGeneration
{
    private const int DirtDeformPass1MaxTimes = 10;
    private const int DirtDeformPass2MaxTimes = 20;
    private const int SandDeformPass1MaxTimes = 15;
    private const int SandDeformPass2MaxTimes = 1000;
    private const int SandDeformPass3MaxTimes = 100;
    
    private const int LakeSeedingPass1MaxTimes = 5;
    private const int RockSeedingPass1MaxTimes = 20;
    
    private const float DirtDeformPass1MaxSizeMultiplier = 0.15f;
    private const float DirtDeformPass2MaxSizeMultiplier = 0.06f;
    private const float SandDeformPass1MaxSizeMultiplier = 0.10f;
    private const float SandDeformPass2MaxSizeMultiplier = 0.02f;
    private const float SandDeformPass3MaxSizeMultiplier = 0.06f;

    private const float LakeSeedingPass1MaxSizeMultiplier = 0.02f;
    private const float RockSeedingPass1MaxSizeMultiplier = 0.05f;
    
    private const float SandPadPass1MaxTimesMultiplier = 0.06f;
    private const float SandPadPass2MaxTimesMultiplier = 0.02f;
    
    private static readonly Random Rnd = new();

    /// <summary>
    ///     Randomly generates a new map and returns it
    /// </summary>
    /// <param name="mapSize"> Integer that represents the width and height of the map we want to generate </param>
    public static GameMap GenerateMap(int mapSize)
    {
        var gameMap = new GameMap(mapSize);

        // Fill island base area with dirt
        gameMap.FillMapSection(gameMap.BaseIslandArea.Start(), gameMap.BaseIslandArea.End(), TileType.Dirt);

        // Dirt perimeter deform pass 1 and 2
        DeformPerimeter(gameMap, DirtDeformPass1MaxTimes, (int)(gameMap.MapSize * DirtDeformPass1MaxSizeMultiplier),
            TileType.Dirt, TileType.Ocean);
        DeformPerimeter(gameMap, DirtDeformPass2MaxTimes, (int)(gameMap.MapSize * DirtDeformPass2MaxSizeMultiplier),
            TileType.Dirt, TileType.Ocean);

        // Dirt padding pass 1
        PadTileTransition(gameMap, TileType.Ocean, TileType.Dirt, TileType.Dirt);

        // Lake seeding pass 1
        SeedTileType(gameMap, LakeSeedingPass1MaxTimes, (int)(gameMap.MapSize * LakeSeedingPass1MaxSizeMultiplier), null, TileType.Lake);

        // Lake padding pass 1
        PadTileTransition(gameMap, TileType.Ocean, TileType.Lake, TileType.Lake);
        PadTileTransition(gameMap, TileType.Dirt, TileType.Lake, TileType.Lake);

        // Sand padding pass 1
        PadTileTransition(gameMap, TileType.Ocean, TileType.Lake, TileType.Sand);
        PadTileTransition(gameMap, TileType.Ocean, TileType.Dirt, TileType.Sand);
        for (var i = 0; i < Rnd.Next((int)(gameMap.MapSize * SandPadPass1MaxTimesMultiplier)); i++)
            PadTileTransition(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);

        // Rock seeding pass 1
        SeedTileType(gameMap, RockSeedingPass1MaxTimes, (int)(gameMap.MapSize * RockSeedingPass1MaxSizeMultiplier), TileType.Dirt, TileType.Rock);

        // Rock padding pass 1
        PadTileTransition(gameMap, TileType.Dirt, TileType.Rock, TileType.Rock);

        // Sand perimeter deform pass 1 and 2
        DeformPerimeter(gameMap, SandDeformPass1MaxTimes, (int)(gameMap.MapSize * SandDeformPass1MaxSizeMultiplier),
            TileType.Sand, TileType.Ocean);
        DeformPerimeter(gameMap, SandDeformPass2MaxTimes, (int)(gameMap.MapSize * SandDeformPass2MaxSizeMultiplier),
            TileType.Sand, TileType.Ocean);

        // Sand padding pass 2
        PadTileTransition(gameMap, TileType.Ocean, TileType.Rock, TileType.Sand);
        PadTileTransition(gameMap, TileType.Ocean, TileType.Dirt, TileType.Sand);
        for (var i = 0; i < Rnd.Next((int)(gameMap.MapSize * SandPadPass2MaxTimesMultiplier)); i++)
            PadTileTransition(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);

        // Sand deform pass 3
        DeformPerimeter(gameMap, SandDeformPass3MaxTimes, (int)(gameMap.MapSize * SandDeformPass3MaxSizeMultiplier),
            TileType.Sand, TileType.Ocean);

        // Sand padding pass 3
        PadTileTransition(gameMap, TileType.Ocean, TileType.Sand, TileType.Sand);

        // River generation pass 1 and pass 2
        RiverGenerator(gameMap);
        RiverGenerator(gameMap);
        
        // River padding pass 1
        PadTileTransition(gameMap, TileType.Dirt, TileType.River, TileType.River);
        PadTileTransition(gameMap, TileType.Rock, TileType.River, TileType.River);
        PadTileTransition(gameMap, TileType.Sand, TileType.River, TileType.River);

        // Ocean padding pass 1, this erodes river tiles that extend out into the ocean
        PadTileTransition(gameMap, TileType.River, TileType.Ocean, TileType.Ocean);
        PadTileTransition(gameMap, TileType.River, TileType.Ocean, TileType.Ocean);

        return gameMap;
    }

    /// <summary>
    ///     Deforms the perimeter of the GameMap's BaseIslandArea by replacing random subsections of it
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to deform </param>
    /// <param name="deformMaxTimes"> Maximum number of times each side should be deformed </param>
    /// <param name="deformMaxSize"> Maximum size of a randomly selected subsection </param>
    /// <param name="replaceTileType"> TileType that should be replaced if located in a subsection </param>
    /// <param name="fillTileType"> TileType the subsections should be filled with </param>
    private static void DeformPerimeter(GameMap gameMap, int deformMaxTimes, int deformMaxSize,
        TileType replaceTileType, TileType fillTileType)
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
    ///     Randomly generates a river that starts on one side of the map and meanders until it hits a map edge
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to add a river to </param>
    private static void RiverGenerator(GameMap gameMap)
    {
        var directionsArray = new[] { Direction.North, Direction.South, Direction.East, Direction.West };
        var riverStartDirection = directionsArray[Rnd.Next(directionsArray.Length)];
        var riverStartTile = riverStartDirection switch
        {
            Direction.North => new Vector2(Rnd.Next(gameMap.MapSize), 0),
            Direction.South => new Vector2(Rnd.Next(gameMap.MapSize), gameMap.MapSize - 1),
            Direction.East => new Vector2(gameMap.MapSize - 1, Rnd.Next(gameMap.MapSize)),
            Direction.West => new Vector2(0, Rnd.Next(gameMap.MapSize)),
            _ => Vector2.One
        };
        var riverCurrentTile = riverStartTile;
        var attemptCounter = 0;

        // Remove direction the river started from list of directions, this prevents the river from flowing backwards
        directionsArray = directionsArray.Where(val => val != riverStartDirection).ToArray();

        // Continuously place river segments until we hit the map edge again
        while (true)
        {
            var tilesPendingUpdate = new List<Vector2>();
            var riverDirection = directionsArray[Rnd.Next(directionsArray.Length)];
            var flowDistance = Rnd.Next(1, gameMap.MapSize / 25);
            attemptCounter++;
            
            // If river generation has not found a map edge after a large number of iterations, return now to stop
            if (attemptCounter > gameMap.MapSize * 10)
            {
                return;
            }

            // Add tiles to list of tiles to be converted to river tiles
            var lastTile = riverCurrentTile;
            for (var i = 0; i < flowDistance; i++)
            {
                var nextTile = lastTile;
                nextTile = riverDirection switch
                {
                    Direction.North => nextTile with { Y = nextTile.Y + 1 },
                    Direction.South => nextTile with { Y = nextTile.Y - 1 },
                    Direction.East => nextTile with { X = nextTile.X + 1 },
                    Direction.West => nextTile with { X = nextTile.X - 1 },
                    _ => nextTile
                };

                if (!gameMap.TileMap.InRange(nextTile.X_int(), nextTile.Y_int())) continue;

                tilesPendingUpdate.Add(nextTile);
                lastTile = nextTile;
            }

            // Set marked tiles to river tiles
            foreach (var tileCoordinate in tilesPendingUpdate)
            {
                gameMap.TileMap[tileCoordinate.X_int(), tileCoordinate.Y_int()] = TileType.River;
                riverCurrentTile = tileCoordinate;

                // Once the river hits a map edge, return to stop river generation
                if (riverCurrentTile.X_int() != riverStartTile.X_int() && riverCurrentTile.Y_int() == riverStartTile.Y_int())
                {
                    if (tileCoordinate.X_int() == 0 || tileCoordinate.X_int() == gameMap.MapSize - 1 ||
                        tileCoordinate.Y_int() == 0 || tileCoordinate.Y_int() == gameMap.MapSize - 1)
                    {
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Randomly seeds a cluster of tiles with different TileType
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to seed tile clusters on </param>
    /// <param name="seedMaxTimes"> Maximum number of seeds that should be added </param>
    /// <param name="seedMaxSize"> Maximum size for seeded clusters </param>
    /// <param name="replaceTileType"> TileType that should be replaced if located in a cluster, can be null to replace anything </param>
    /// <param name="fillTileType"> TileType the clusters should be filled with </param>
    private static void SeedTileType(GameMap gameMap, int seedMaxTimes, int seedMaxSize, TileType? replaceTileType,
        TileType fillTileType)
    {
        var start = gameMap.BaseIslandArea.Start();
        var end = gameMap.BaseIslandArea.End();

        for (var i = 0; i < Rnd.Next(seedMaxTimes); i++)
        {
            var seedCoordinate =
                new Vector2(Rnd.Next(start.X_int(), end.X_int()), Rnd.Next(start.Y_int(), end.Y_int()));
            var seedStartPoints = new List<Vector2>
            {
                seedCoordinate,
                new(seedCoordinate.X_int() + Rnd.Next(-2, 2), seedCoordinate.Y_int() + Rnd.Next(-2, 2)),
                new(seedCoordinate.X_int() + Rnd.Next(-5, 5), seedCoordinate.Y_int() + Rnd.Next(-5, 5))
            };

            foreach (var seedStartPoint in seedStartPoints)
                for (var mapX = seedStartPoint.X_int() - seedMaxSize;
                     mapX < seedStartPoint.X_int() + seedMaxSize;
                     mapX++)
                for (var mapY = seedStartPoint.Y_int() - seedMaxSize;
                     mapY < seedStartPoint.Y_int() + seedMaxSize;
                     mapY++)
                    if (gameMap.BaseIslandArea.PointInsideRectangle(new Vector2(mapX, mapY)))
                    {
                        if (replaceTileType != null && gameMap.TileMap[mapX, mapY] != replaceTileType) continue;

                        gameMap.TileMap[mapX, mapY] = fillTileType;
                    }
        }
    }

    /// <summary>
    ///     Pads transitions between TileTypes
    /// </summary>
    /// <param name="gameMap"> GameMap that we want to pad </param>
    /// <param name="tileType1"> TileType that we are checking for as the transition, replaced if valid </param>
    /// <param name="tileType2"> TileType that we are checking for as the edge </param>
    /// <param name="fill"> TileType that valid transition tiles should be replaced with </param>
    private static void PadTileTransition(GameMap gameMap, TileType tileType1, TileType tileType2, TileType fill)
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