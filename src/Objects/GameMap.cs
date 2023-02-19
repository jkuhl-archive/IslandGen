using System.Numerics;
using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects;

public class GameMap
{
    public const int MapSize = 100;
    public const int MapBuffer = MapSize / 10;
    public const int TileTextureSize = 16;

    private const int DirtDeform1Iterations = 10;
    private const int DirtDeform2Iterations = 20;
    private const int LakeSeeding1Iterations = 5;
    private const int RockSeeding1Iterations = 20;
    private const int SandDeform1Iterations = 15;
    private const int SandDeform2Iterations = 1000;
    private const int SandDeform3Iterations = 100;
    private const int VegetationDenseSeeding1Iterations = 10;
    private const int VegetationGrowthAmount = 20;
    private const int VegetationModerateSeeding1Iterations = 10;
    private const int VegetationSparseSeeding1Iterations = 50;
    private const float DirtDeform1Multiplier = 0.15f;
    private const float DirtDeform2Multiplier = 0.06f;
    private const float LakeSeeding1Multiplier = 0.02f;
    private const float RockSeeding1Multiplier = 0.05f;
    private const float SandDeform1Multiplier = 0.10f;
    private const float SandDeform2Multiplier = 0.02f;
    private const float SandDeform3Multiplier = 0.06f;
    private const float SandPad1Multiplier = 0.06f;
    private const float SandPad2Multiplier = 0.02f;
    private const float VegetationDenseSeeding1Multiplier = 0.1f;
    private const float VegetationModerateSeeding1Multiplier = 0.05f;
    private const float VegetationSparseSeeding1Multiplier = 0.1f;

    [JsonIgnore] private readonly Rectangle _baseIslandArea =
        new(MapBuffer, MapBuffer, MapSize - MapBuffer * 2, MapSize - MapBuffer * 2);

    [JsonProperty] private readonly TileType[,] _tileMap = new TileType[MapSize, MapSize];

    public void Draw()
    {
        // TODO: Implement culling to prevent wasted time drawing stuff off screen
        for (var mapX = 0; mapX < MapSize; mapX++)
        for (var mapY = 0; mapY < MapSize; mapY++)
        {
            var currentTile = _tileMap[mapX, mapY];
            if (currentTile.IsAnimated())
            {
                var texture = Assets.AnimatedTextures[currentTile.GetTileTextureName()];
                texture.Draw(GetTileCoordinates((mapX, mapY)));
            }
            else
            {
                var texture = Assets.Textures[currentTile.GetTileTextureName()];
                Raylib.DrawTexture(texture, mapX * TileTextureSize, mapY * TileTextureSize, Raylib.WHITE);
            }
        }
    }

    public void Update()
    {
        VegetationGrowth();
    }

    /// <summary>
    ///     Randomly generates an island on the game map
    /// </summary>
    public void GenerateMap()
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

        // Vegetation sparse and vegetation moderate seeding pass 1
        SeedTileType(VegetationSparseSeeding1Iterations, VegetationSparseSeeding1Multiplier,
            TileType.Dirt, TileType.VegetationSparse);
        SeedTileType(VegetationModerateSeeding1Iterations, VegetationModerateSeeding1Multiplier,
            TileType.VegetationSparse, TileType.VegetationModerate);

        // Vegetation sparse and vegetation moderate padding pass 1
        PadTileTransition(TileType.Dirt, TileType.VegetationSparse, TileType.VegetationSparse);
        PadTileTransition(TileType.Dirt, TileType.VegetationSparse, TileType.VegetationSparse);
        PadTileTransition(TileType.Dirt, TileType.VegetationSparse, TileType.VegetationSparse);
        PadTileTransition(TileType.Dirt, TileType.VegetationModerate, TileType.VegetationModerate);
        PadTileTransition(TileType.VegetationSparse, TileType.VegetationModerate, TileType.VegetationModerate);
        PadTileTransition(TileType.VegetationSparse, TileType.VegetationModerate, TileType.VegetationModerate);

        // Vegetation dense seeding pass 1
        SeedTileType(VegetationDenseSeeding1Iterations, VegetationDenseSeeding1Multiplier,
            TileType.VegetationSparse, TileType.VegetationDense);
        SeedTileType(VegetationDenseSeeding1Iterations, VegetationDenseSeeding1Multiplier,
            TileType.VegetationModerate, TileType.VegetationDense);

        // Vegetation dense padding pass 1
        PadTileTransition(TileType.Dirt, TileType.VegetationDense, TileType.VegetationDense);
        PadTileTransition(TileType.VegetationSparse, TileType.VegetationDense, TileType.VegetationDense);
        PadTileTransition(TileType.VegetationModerate, TileType.VegetationDense, TileType.VegetationDense);
        PadTileTransition(TileType.VegetationModerate, TileType.VegetationDense, TileType.VegetationDense);
    }

    /// <summary>
    ///     Gets the positions the camera should stop at to prevent panning off the of the game map
    /// </summary>
    /// <returns> Tuple containing the maximum positions the camera can pan to </returns>
    public (int, int) GetCameraPanLimits()
    {
        var camera = ServiceManager.GetService<GameLogic>().GameCamera.Camera;
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var cameraViewWidth = MapSize * TileTextureSize * camera.zoom * scalingManager.ScaleFactor;
        var cameraViewHeight = MapSize * TileTextureSize * camera.zoom * scalingManager.ScaleFactor;
        var mapViewWidth = MapSize * scalingManager.WindowWidth / cameraViewWidth;
        var mapViewHeight = MapSize * scalingManager.WindowHeight / cameraViewHeight;

        return (
            (int)((MapSize - mapViewWidth) * TileTextureSize),
            (int)((MapSize - mapViewHeight) * TileTextureSize));
    }

    /// <summary>
    ///     Gets the position of the mouse cursor on the game map
    /// </summary>
    /// <returns> Vector2 containing the mouse cursor's position on the game map </returns>
    public Vector2 GetMapMousePosition()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var mousePosition = InputManager.GetMousePosition() / scalingManager.ScaleFactor;
        return Raylib.GetScreenToWorld2D(mousePosition, gameLogic.GameCamera.Camera);
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
            if (!allowWater && _tileMap[position.Item1, position.Item2].IsWater()) continue;

            return position;
        }
    }

    /// <summary>
    ///     Gets the TileType of the tile at the given position on the game map
    /// </summary>
    /// <param name="tilePosition"> Tuple containing position of tile </param>
    /// <returns> TileType of the given tile </returns>
    public TileType GetTileType((int, int) tilePosition)
    {
        return _tileMap[tilePosition.Item1, tilePosition.Item2];
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
    ///     Returns the texture size of tiles on the game map
    /// </summary>
    /// <returns> Int that represents the width and height of tiles on game map </returns>
    public int GetTileTextureSize()
    {
        return TileTextureSize;
    }

    /// <summary>
    ///     Gets a list of tiles on the game map that match the given TileType
    /// </summary>
    /// <param name="tileType"> TileType of the tiles we want a list of </param>
    /// <returns> List of tuples containing the X and Y positions of map tiles with the given TileType </returns>
    public List<(int, int)> GetTileTypeList(TileType tileType)
    {
        var tileList = new List<(int, int)>();
        for (var mapX = 0; mapX < MapSize; mapX++)
        for (var mapY = 0; mapY < MapSize; mapY++)
            if (_tileMap[mapX, mapY] == tileType)
                tileList.Add((mapX, mapY));

        return tileList;
    }

    /// <summary>
    ///     Determines the tiles on the map that the camera can currently see
    /// </summary>
    /// <returns> Rectangle that represents the tiles on the map that the camera can currently see </returns>
    public Rectangle GetVisibleMapTiles()
    {
        var camera = ServiceManager.GetService<GameLogic>().GameCamera.Camera;
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var cameraViewWidth = MapSize * TileTextureSize * camera.zoom * scalingManager.ScaleFactor;
        var cameraViewHeight = MapSize * TileTextureSize * camera.zoom * scalingManager.ScaleFactor;

        var mapViewX = (int)Math.Round(camera.target.X / TileTextureSize);
        var mapViewY = (int)Math.Round(camera.target.Y / TileTextureSize);
        var mapViewWidth = (int)Math.Round(MapSize * scalingManager.WindowWidth / cameraViewWidth);
        var mapViewHeight = (int)Math.Round(MapSize * scalingManager.WindowHeight / cameraViewHeight);

        return new Rectangle(mapViewX, mapViewY, mapViewWidth, mapViewHeight);
    }

    /// <summary>
    ///     Checks if the given tile position is in the range of the game map
    /// </summary>
    /// <param name="tilePosition"> Tuple containing position of tile </param>
    /// <returns> True if in range, false if not </returns>
    public bool PositionInRange((int, int) tilePosition)
    {
        return _tileMap.InRange(tilePosition);
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
                    if (_tileMap.InRange(mapX, mapY) && _tileMap[mapX, mapY] == replaceTileType)
                        _tileMap[mapX, mapY] = fillTileType;
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
            if (_tileMap.InRange(mapX, mapY))
                _tileMap[mapX, mapY] = fill;
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
            var currentTile = _tileMap[mapX, mapY];
            var adjacentTiles = new List<(int, int)>
            {
                (mapX, mapY + 1),
                (mapX, mapY - 1),
                (mapX + 1, mapY),
                (mapX - 1, mapY)
            };

            foreach (var adjacentTile in adjacentTiles)
                if (_tileMap.InRange(adjacentTile.Item1, adjacentTile.Item2) &&
                    currentTile == tileType1 &&
                    _tileMap[adjacentTile.Item1, adjacentTile.Item2] == tileType2)
                {
                    tilesPendingUpdate.Add((mapX, mapY));
                    break;
                }
        }

        // Update all marked tiles
        foreach (var tileCoordinates in tilesPendingUpdate)
            _tileMap[tileCoordinates.Item1, tileCoordinates.Item2] = fill;
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

        // Remove direction the river started from list of directions, this prevents the river from flowing backwards
        directionsArray = directionsArray.Where(val => val != riverStartDirection).ToArray();

        // Continuously place river segments until we hit the map edge again
        for (var attempt = 0; attempt < MapSize * 10; attempt++)
        {
            var tilesPendingUpdate = new List<Vector2>();
            var riverDirection = directionsArray[rnd.Next(directionsArray.Length)];
            var flowDistance = rnd.Next(1, MapSize / 25);

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

                if (!_tileMap.InRange(nextTile.X_int(), nextTile.Y_int())) continue;

                tilesPendingUpdate.Add(nextTile);
                lastTile = nextTile;
            }

            // Set marked tiles to river tiles
            foreach (var tileCoordinate in tilesPendingUpdate)
            {
                _tileMap[tileCoordinate.X_int(), tileCoordinate.Y_int()] = TileType.River;
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
                        if (replaceTileType != null && _tileMap[mapX, mapY] != replaceTileType) continue;
                        _tileMap[mapX, mapY] = fillTileType;
                    }
        }
    }

    /// <summary>
    ///     Spreads vegetation across the game map
    /// </summary>
    private void VegetationGrowth()
    {
        var rnd = ServiceManager.GetService<Random>();

        for (var i = 0; i < rnd.Next(MapSize / VegetationGrowthAmount); i++)
        {
            var startTile = GetRandomTile(true);
            var startTileType = _tileMap[startTile.Item1, startTile.Item2];
            var adjacentTiles = new List<(int, int)>
            {
                startTile,
                (startTile.Item1, startTile.Item2 + 1),
                (startTile.Item1, startTile.Item2 - 1),
                (startTile.Item1 + 1, startTile.Item2),
                (startTile.Item1 - 1, startTile.Item2)
            };

            // If start tile and all adjacent tiles contain vegetation, bump up the start tile's vegetation density
            if (adjacentTiles.TrueForAll(tile =>
                    _tileMap[tile.Item1, tile.Item2] is TileType.VegetationSparse or TileType.VegetationModerate
                        or TileType.VegetationDense))
            {
                switch (startTileType)
                {
                    case TileType.VegetationSparse:
                        _tileMap[startTile.Item1, startTile.Item2] = TileType.VegetationModerate;
                        break;
                    case TileType.VegetationModerate:
                        _tileMap[startTile.Item1, startTile.Item2] = TileType.VegetationDense;
                        break;
                }

                return;
            }

            // If start tile contains sparse vegetation, lake, or river replace adjacent dirt with sparse vegetation
            if (startTileType is TileType.VegetationSparse or TileType.Lake or TileType.River)
                foreach (var tile in adjacentTiles.Where(adjacentTile =>
                             _tileMap[adjacentTile.Item1, adjacentTile.Item2] == TileType.Dirt))
                    _tileMap[tile.Item1, tile.Item2] = TileType.VegetationSparse;
        }
    }
}