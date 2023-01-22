using System.Numerics;
using IslandGen.Data;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameMap
{
    // Map generation variables
    private const int DirtDeform1Iterations = 10;
    private const int DirtDeform2Iterations = 20;
    private const int SandDeform1Iterations = 15;
    private const int SandDeform2Iterations = 1000;
    private const int SandDeform3Iterations = 100;
    private const int LakeSeeding1Iterations = 5;
    private const int RockSeeding1Iterations = 20;

    private const float DirtDeform1Multiplier = 0.15f;
    private const float DirtDeform2Multiplier = 0.06f;
    private const float SandDeform1Multiplier = 0.10f;
    private const float SandDeform2Multiplier = 0.02f;
    private const float SandDeform3Multiplier = 0.06f;
    private const float LakeSeeding1Multiplier = 0.02f;
    private const float RockSeeding1Multiplier = 0.05f;
    private const float SandPad1Multiplier = 0.06f;
    private const float SandPad2Multiplier = 0.02f;

    public readonly Rectangle BaseIslandArea;
    public readonly int MapBuffer;
    public readonly int MapSize;
    public readonly RenderTexturePro MapTexture;
    public readonly TileType[,] TileMap;
    public readonly int TileTextureSize;

    /// <summary>
    ///     Constructor for GameMap
    /// </summary>
    public GameMap(int mapSize = 100, int tileTextureSize = 16)
    {
        MapSize = mapSize;
        MapBuffer = MapSize / 10;
        TileTextureSize = tileTextureSize;

        BaseIslandArea = new Rectangle(MapBuffer, MapBuffer, MapSize - MapBuffer * 2, MapSize - MapBuffer * 2);
        MapTexture = new RenderTexturePro(new Vector2(MapSize * TileTextureSize, MapSize * TileTextureSize));
        TileMap = new TileType[MapSize, MapSize];

        GenerateMap();
    }

    public void Draw()
    {
        Raylib.BeginTextureMode(MapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        Raylib.BeginMode2D(ServiceManager.GetService<GameCamera>().Camera2D);
        for (var mapX = 0; mapX < MapSize; mapX++)
        for (var mapY = 0; mapY < MapSize; mapY++)
        {
            var texture = ServiceManager.GetService<TextureManager>()
                .Textures[TileMap[mapX, mapY].GetTileTextureName()];
            Raylib.DrawTexture(texture, mapX * TileTextureSize, mapY * TileTextureSize, Raylib.WHITE);
        }

        ServiceManager.GetService<EntityManager>().Draw();
        Raylib.EndMode2D();
        Raylib.EndTextureMode();

        MapTexture.Draw();
    }

    /// <summary>
    ///     Randomly generates a new map and returns it
    /// </summary>
    private void GenerateMap()
    {
        var rnd = ServiceManager.GetService<Random>();

        // Fill map with ocean, then fill island base area with dirt
        FillMapSection(Vector2.Zero, new Vector2(MapSize), TileType.Ocean);
        FillMapSection(BaseIslandArea.Start(), BaseIslandArea.End(), TileType.Dirt);

        // Dirt perimeter deform pass 1 and 2
        DeformPerimeter(DirtDeform1Iterations, DirtDeform1Multiplier, TileType.Dirt, TileType.Ocean);
        DeformPerimeter(DirtDeform2Iterations, DirtDeform2Multiplier, TileType.Dirt, TileType.Ocean);

        // Dirt padding pass 1
        PadTileTransition(TileType.Ocean, TileType.Dirt, TileType.Dirt);

        // Lake seeding pass 1
        SeedTileType(LakeSeeding1Iterations, LakeSeeding1Multiplier, null, TileType.Lake);

        // Lake padding pass 1
        PadTileTransition(TileType.Ocean, TileType.Lake, TileType.Lake);
        PadTileTransition(TileType.Dirt, TileType.Lake, TileType.Lake);

        // Sand padding pass 1
        PadTileTransition(TileType.Ocean, TileType.Lake, TileType.Sand);
        PadTileTransition(TileType.Ocean, TileType.Dirt, TileType.Sand);
        for (var i = 0; i < rnd.Next((int)(MapSize * SandPad1Multiplier)); i++)
            PadTileTransition(TileType.Ocean, TileType.Sand, TileType.Sand);

        // Rock seeding pass 1
        SeedTileType(RockSeeding1Iterations, RockSeeding1Multiplier, TileType.Dirt, TileType.Rock);

        // Rock padding pass 1
        PadTileTransition(TileType.Dirt, TileType.Rock, TileType.Rock);

        // Sand perimeter deform pass 1 and 2
        DeformPerimeter(SandDeform1Iterations, SandDeform1Multiplier, TileType.Sand, TileType.Ocean);
        DeformPerimeter(SandDeform2Iterations, SandDeform2Multiplier, TileType.Sand, TileType.Ocean);

        // Sand padding pass 2
        PadTileTransition(TileType.Ocean, TileType.Rock, TileType.Sand);
        PadTileTransition(TileType.Ocean, TileType.Dirt, TileType.Sand);
        for (var i = 0; i < rnd.Next((int)(MapSize * SandPad2Multiplier)); i++)
            PadTileTransition(TileType.Ocean, TileType.Sand, TileType.Sand);

        // Sand deform pass 3
        DeformPerimeter(SandDeform3Iterations, SandDeform3Multiplier, TileType.Sand, TileType.Ocean);

        // Sand padding pass 3
        PadTileTransition(TileType.Ocean, TileType.Sand, TileType.Sand);

        // River generation pass 1 and pass 2
        RiverGenerator();
        RiverGenerator();

        // River padding pass 1
        PadTileTransition(TileType.Dirt, TileType.River, TileType.River);
        PadTileTransition(TileType.Rock, TileType.River, TileType.River);
        PadTileTransition(TileType.Sand, TileType.River, TileType.River);

        // Ocean padding pass 1, this erodes river tiles that extend out into the ocean
        PadTileTransition(TileType.River, TileType.Ocean, TileType.Ocean);
        PadTileTransition(TileType.River, TileType.Ocean, TileType.Ocean);
    }

    /// <summary>
    ///     Deforms the perimeter of the GameMap's BaseIslandArea by replacing random subsections of it
    /// </summary>
    /// <param name="maxIterations"> Max number of time deform workflow should run </param>
    /// <param name="sizeMultiplier"> Multiplier used to scale size of deform sections based on MapSize </param>
    /// <param name="replaceTileType"> TileType that should be replaced if located in a subsection </param>
    /// <param name="fillTileType"> TileType the subsections should be filled with </param>
    private void DeformPerimeter(int maxIterations, float sizeMultiplier, TileType replaceTileType,
        TileType fillTileType)
    {
        var rnd = ServiceManager.GetService<Random>();
        var start = BaseIslandArea.Start();
        var end = BaseIslandArea.End();

        for (var i = 0; i < rnd.Next(maxIterations); i++)
        {
            var deformStartPoints = new List<Vector2>
            {
                start with { X = rnd.Next(start.X_int(), end.X_int()) },
                start with { Y = rnd.Next(start.Y_int(), end.Y_int()) },
                end with { X = rnd.Next(start.X_int(), end.X_int()) },
                end with { Y = rnd.Next(start.Y_int(), end.Y_int()) }
            };

            foreach (var deformStart in deformStartPoints)
            {
                var deformSize = rnd.Next((int)(MapSize * sizeMultiplier));

                for (var mapX = deformStart.X_int() - deformSize; mapX < deformStart.X_int() + deformSize; mapX++)
                for (var mapY = deformStart.Y_int() - deformSize; mapY < deformStart.Y_int() + deformSize; mapY++)
                    if (TileMap.InRange(mapX, mapY) && TileMap[mapX, mapY] == replaceTileType)
                        TileMap[mapX, mapY] = fillTileType;
            }
        }
    }

    /// <summary>
    ///     Fills a section of the map
    /// </summary>
    /// <param name="start"> Starting point for the fill operation (top left corner) </param>
    /// <param name="end"> Ending point for the fill operation (bottom right corner) </param>
    /// <param name="fill"> TileType the section should be filled with </param>
    private void FillMapSection(Vector2 start, Vector2 end, TileType fill)
    {
        for (var mapX = start.X_int(); mapX < end.X_int(); mapX++)
        for (var mapY = start.Y_int(); mapY < end.Y_int(); mapY++)
            if (TileMap.InRange(mapX, mapY))
                TileMap[mapX, mapY] = fill;
    }

    /// <summary>
    ///     Randomly generates a river that starts on one side of the map and meanders until it hits a map edge
    /// </summary>
    private void RiverGenerator()
    {
        var rnd = ServiceManager.GetService<Random>();
        var directionsArray = new[] { Direction.North, Direction.South, Direction.East, Direction.West };
        var riverStartDirection = directionsArray[rnd.Next(directionsArray.Length)];
        var riverStartTile = riverStartDirection switch
        {
            Direction.North => new Vector2(rnd.Next(MapSize), 0),
            Direction.South => new Vector2(rnd.Next(MapSize), MapSize - 1),
            Direction.East => new Vector2(MapSize - 1, rnd.Next(MapSize)),
            Direction.West => new Vector2(0, rnd.Next(MapSize)),
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
            var riverDirection = directionsArray[rnd.Next(directionsArray.Length)];
            var flowDistance = rnd.Next(1, MapSize / 25);
            attemptCounter++;

            // If river generation has not found a map edge after a large number of iterations, return now to stop
            if (attemptCounter > MapSize * 10) return;

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

                if (!TileMap.InRange(nextTile.X_int(), nextTile.Y_int())) continue;

                tilesPendingUpdate.Add(nextTile);
                lastTile = nextTile;
            }

            // Set marked tiles to river tiles
            foreach (var tileCoordinate in tilesPendingUpdate)
            {
                TileMap[tileCoordinate.X_int(), tileCoordinate.Y_int()] = TileType.River;
                riverCurrentTile = tileCoordinate;

                // Once the river hits a map edge, return to stop river generation
                if (riverCurrentTile.X_int() != riverStartTile.X_int() &&
                    riverCurrentTile.Y_int() == riverStartTile.Y_int())
                    if (tileCoordinate.X_int() == 0 || tileCoordinate.X_int() == MapSize - 1 ||
                        tileCoordinate.Y_int() == 0 || tileCoordinate.Y_int() == MapSize - 1)
                        return;
            }
        }
    }

    /// <summary>
    ///     Randomly seeds a cluster of tiles with different TileType
    /// </summary>
    /// <param name="maxIterations"> Maximum number of seeds that should be added </param>
    /// <param name="sizeMultiplier"> Maximum size for seeded clusters </param>
    /// <param name="replaceTileType"> TileType that should be replaced, can be null to replace anything </param>
    /// <param name="fillTileType"> TileType the clusters should be filled with </param>
    private void SeedTileType(int maxIterations, float sizeMultiplier, TileType? replaceTileType, TileType fillTileType)
    {
        var rnd = ServiceManager.GetService<Random>();
        var start = BaseIslandArea.Start();
        var end = BaseIslandArea.End();

        for (var i = 0; i < rnd.Next(maxIterations); i++)
        {
            var seedCoordinate =
                new Vector2(rnd.Next(start.X_int(), end.X_int()), rnd.Next(start.Y_int(), end.Y_int()));
            var seedStartPoints = new List<Vector2>
            {
                seedCoordinate,
                new(seedCoordinate.X_int() + rnd.Next(-2, 2), seedCoordinate.Y_int() + rnd.Next(-2, 2)),
                new(seedCoordinate.X_int() + rnd.Next(-5, 5), seedCoordinate.Y_int() + rnd.Next(-5, 5))
            };

            foreach (var seedStartPoint in seedStartPoints)
                for (var mapX = seedStartPoint.X_int() - (int)(MapSize * sizeMultiplier);
                     mapX < seedStartPoint.X_int() + (int)(MapSize * sizeMultiplier);
                     mapX++)
                for (var mapY = seedStartPoint.Y_int() - (int)(MapSize * sizeMultiplier);
                     mapY < seedStartPoint.Y_int() + (int)(MapSize * sizeMultiplier);
                     mapY++)
                    if (BaseIslandArea.PointInsideRectangle(new Vector2(mapX, mapY)))
                    {
                        if (replaceTileType != null && TileMap[mapX, mapY] != replaceTileType) continue;
                        TileMap[mapX, mapY] = fillTileType;
                    }
        }
    }

    /// <summary>
    ///     Pads transitions between TileTypes
    /// </summary>
    /// <param name="tileType1"> TileType that we are checking for as the transition, replaced if valid </param>
    /// <param name="tileType2"> TileType that we are checking for as the edge </param>
    /// <param name="fill"> TileType that valid transition tiles should be replaced with </param>
    private void PadTileTransition(TileType tileType1, TileType tileType2, TileType fill)
    {
        var tilesPendingUpdate = new List<(int, int)>();

        // Find tiles that exist at the edge of tileType1 and tileType2 and mark them to be updated
        for (var mapX = 0; mapX < MapSize; mapX++)
        for (var mapY = 0; mapY < MapSize; mapY++)
        {
            var currentTile = TileMap[mapX, mapY];
            var adjacentTiles = new List<(int, int)>
            {
                (mapX, mapY + 1),
                (mapX, mapY - 1),
                (mapX + 1, mapY),
                (mapX - 1, mapY)
            };

            foreach (var adjacentTile in adjacentTiles)
                if (TileMap.InRange(adjacentTile.Item1, adjacentTile.Item2) &&
                    currentTile == tileType1 &&
                    TileMap[adjacentTile.Item1, adjacentTile.Item2] == tileType2)
                {
                    tilesPendingUpdate.Add((mapX, mapY));
                    break;
                }
        }

        // Update all marked tiles
        foreach (var tileCoordinates in tilesPendingUpdate)
            TileMap[tileCoordinates.Item1, tileCoordinates.Item2] = fill;
    }
}