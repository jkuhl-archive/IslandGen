using System.Numerics;
using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Data.Textures;
using IslandGen.Extensions;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameMap
{
    private const int MapSize = 100;
    private const int MapBuffer = MapSize / 10;
    private const int TileTextureSize = 16;

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

    private readonly Rectangle _baseIslandArea =
        new(MapBuffer, MapBuffer, MapSize - MapBuffer * 2, MapSize - MapBuffer * 2);

    private readonly RenderTexturePro _mapTexture =
        new(new Vector2(MapSize * TileTextureSize, MapSize * TileTextureSize));

    public readonly TileType[,] TileMap;

    /// <summary>
    ///     Service that manages the game's map and entities
    /// </summary>
    public GameMap()
    {
        TileMap = new TileType[MapSize, MapSize];
        GenerateMap();
    }

    /// <summary>
    ///     Constructor for loading a saved GameMap
    /// </summary>
    /// <param name="tileMap"> Array that stores the map's tiles </param>
    [JsonConstructor]
    private GameMap(TileType[,] tileMap)
    {
        TileMap = tileMap;
    }

    public void Draw()
    {
        var gameCamera = ServiceManager.GetService<GameCamera>();
        var gameSettings = ServiceManager.GetService<GameSettings>();
        var textureManager = ServiceManager.GetService<TextureManager>();
        var visibleArea = GetVisibleMapArea();

        // Begin rendering map to texture
        Raylib.BeginTextureMode(_mapTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        Raylib.BeginMode2D(gameCamera.Camera);

        // Draw tiles
        // TODO: This can be optimized to use the visible area as the for loop start and end
        for (var mapX = 0; mapX < MapSize; mapX++)
        for (var mapY = 0; mapY < MapSize; mapY++)
        {
            if (!visibleArea.PointInsideRectangle(mapX, mapY)) continue;

            var currentTile = TileMap[mapX, mapY];
            if (currentTile.IsAnimated())
            {
                var texture = textureManager.AnimatedTextures[currentTile.GetTileTextureName()];
                texture.Draw(GetTileCoordinates((mapX, mapY)));
            }
            else
            {
                var texture = textureManager.Textures[currentTile.GetTileTextureName()];
                Raylib.DrawTexture(texture, mapX * TileTextureSize, mapY * TileTextureSize, Raylib.WHITE);
            }
        }

        // Draw entities
        ServiceManager.GetService<GameLogic>().Draw();

        // Draw debug elements
        if (gameSettings.DebugMode)
        {
            var mapMouseTile = GetMapMouseTile();

            // Draw grid
            for (var mapX = 1; mapX < MapSize; mapX++)
            {
                var x = mapX * TileTextureSize;
                Raylib.DrawLine(x, 0, x, _mapTexture.RenderTexture.texture.height, Colors.TransparentGray);
            }

            for (var mapY = 1; mapY < MapSize; mapY++)
            {
                var y = mapY * TileTextureSize;
                Raylib.DrawLine(0, y, _mapTexture.RenderTexture.texture.width, y, Colors.TransparentGray);
            }

            if (_mapTexture.DestinationRectangle.PointInsideRectangle(GetMapMousePosition()))
            {
                // Draw box around highlighted tile
                Raylib.DrawRectangleLines(
                    mapMouseTile.Item1 * TileTextureSize,
                    mapMouseTile.Item2 * TileTextureSize,
                    TileTextureSize,
                    TileTextureSize,
                    Raylib.RED
                );

                // Draw a dot that marks the mouse cursors position on the map
                Raylib.DrawCircleV(GetMapMousePosition(), 2.0f, Raylib.RED);
            }
        }

        // Stop rendering to texture
        Raylib.EndMode2D();
        Raylib.EndTextureMode();

        // Draw texture to screen
        _mapTexture.Draw(true);
    }

    /// <summary>
    ///     Returns the size of the map
    /// </summary>
    /// <returns> Int that represents the width and height of the game map </returns>
    public int GetMapSize()
    {
        return MapSize;
    }

    /// <summary>
    ///     Gets the position of the mouse cursor on the game map
    /// </summary>
    /// <returns> Vector2 containing the mouse cursor's position on the game map </returns>
    public Vector2 GetMapMousePosition()
    {
        var gameCamera = ServiceManager.GetService<GameCamera>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var mousePosition = Raylib.GetMousePosition() / scalingManager.ScaleFactor;
        return Raylib.GetScreenToWorld2D(mousePosition, gameCamera.Camera);
    }

    /// <summary>
    ///     Gets the map tile that the mouse cursor is currently above
    /// </summary>
    /// <returns> Tuple containing the X and Y position of the tile the mouse is above </returns>
    public (int, int) GetMapMouseTile()
    {
        var position = GetMapMousePosition();
        return ((int)position.X / TileTextureSize, (int)position.Y / TileTextureSize);
    }

    /// <summary>
    ///     Randomly selects a tile on the game map
    /// </summary>
    /// <param name="allowWater"> If true water tiles can be selected, if false only return land tiles </param>
    /// <returns> Tuple containing position of selected tile </returns>
    public (int, int) GetRandomTile(bool allowWater = false)
    {
        var rnd = ServiceManager.GetService<Random>();

        while (true)
        {
            var position = (rnd.Next(MapSize), rnd.Next(MapSize));
            if (!allowWater && TileMap[position.Item1, position.Item2].IsWater()) continue;

            return position;
        }
    }

    /// <summary>
    ///     Gets the screen coordinates of a tile on the game map
    /// </summary>
    /// <param name="tilePosition"> Tuple containing position of tile </param>
    /// <returns> Vector2 containing the X and Y coordinates of the tile on screen </returns>
    public Vector2 GetTileCoordinates((int, int) tilePosition)
    {
        return new Vector2(tilePosition.Item1 * TileTextureSize, tilePosition.Item2 * TileTextureSize);
    }

    /// <summary>
    ///     Determines the area of the map that the camera can currently see
    /// </summary>
    /// <returns> Rectangle that represents the area of the map that the camera can currently see </returns>
    public Rectangle GetVisibleMapArea()
    {
        var camera = ServiceManager.GetService<GameCamera>().Camera;
        var scalingManager = ServiceManager.GetService<ScalingManager>();

        var cameraViewWidth = _mapTexture.RenderTexture.texture.width * camera.zoom * scalingManager.ScaleFactor;
        var cameraViewHeight = _mapTexture.RenderTexture.texture.height * camera.zoom * scalingManager.ScaleFactor;
        var mapViewX = (int)Math.Round(camera.target.X / TileTextureSize);
        var mapViewY = (int)Math.Round(camera.target.Y / TileTextureSize);
        var mapViewWidth = (int)Math.Round(MapSize * scalingManager.WindowWidth / cameraViewWidth);
        var mapViewHeight = (int)Math.Round(MapSize * scalingManager.WindowHeight / cameraViewHeight);

        return new Rectangle(mapViewX, mapViewY, mapViewWidth, mapViewHeight);
    }

    /// <summary>
    ///     Randomly generates a new map and returns it
    /// </summary>
    private void GenerateMap()
    {
        var rnd = ServiceManager.GetService<Random>();

        // Fill map with ocean, then fill island base area with dirt
        FillMapSection(Vector2.Zero, new Vector2(MapSize), TileType.Ocean);
        FillMapSection(_baseIslandArea.Start(), _baseIslandArea.End(), TileType.Dirt);

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
        var start = _baseIslandArea.Start();
        var end = _baseIslandArea.End();

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
        var start = _baseIslandArea.Start();
        var end = _baseIslandArea.End();

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
                    if (_baseIslandArea.PointInsideRectangle(mapX, mapY))
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