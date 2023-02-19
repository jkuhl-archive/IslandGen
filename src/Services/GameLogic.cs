using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.Objects;
using IslandGen.Objects.ECS;
using IslandGen.Objects.ECS.Entities.Structures;
using IslandGen.Objects.Textures;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    [JsonProperty] private readonly Dictionary<Type, List<EntityBase>> _entities = new();

    [JsonIgnore] private readonly RenderTexturePro _gameWorldTexture =
        new((GameMap.MapSize * GameMap.TileTextureSize, GameMap.MapSize * GameMap.TileTextureSize));

    [JsonProperty] private float _updateTimer;
    [JsonProperty] public DateTime StartDateTime { get; init; }
    [JsonIgnore] public EntityBase? SelectedEntity { get; private set; }
    [JsonProperty] public bool GamePaused { get; private set; }
    [JsonIgnore] public StructureBase? MouseStructure { get; private set; }
    [JsonProperty] public DateTime CurrentDateTime { get; private set; }
    [JsonProperty] public GameSpeed GameSpeed { get; private set; } = GameSpeed.Normal;
    [JsonProperty] public GameCamera GameCamera { get; private init; } = new();
    [JsonProperty] public GameMap GameMap { get; private init; } = new();

    public void Draw()
    {
        var gameSettings = ServiceManager.GetService<GameSettings>();

        // Begin rendering game world to texture
        Raylib.BeginTextureMode(_gameWorldTexture.RenderTexture);
        Raylib.ClearBackground(Raylib.BLACK);
        Raylib.BeginMode2D(GameCamera.Camera);

        // Draw map
        GameMap.Draw();

        // Draw entities
        // TODO: Add map culling here
        foreach (var entity in _entities.Values.SelectMany(entityList => entityList))
            entity.Draw();

        // Draw box around selected entity
        if (SelectedEntity != null)
            Raylib.DrawRectangleLinesEx(SelectedEntity.GetMapSpaceRectangle(), 1, Colors.Selected);
        MouseStructure?.Draw();

        // Draw debug elements
        if (gameSettings.DebugMode)
        {
            // Draw grid
            for (var mapX = 1; mapX < GameMap.MapSize; mapX++)
            {
                var x = mapX * GameMap.TileTextureSize;
                Raylib.DrawLine(x, 0, x, _gameWorldTexture.RenderTexture.texture.height, Colors.GridLine);
            }

            for (var mapY = 1; mapY < GameMap.MapSize; mapY++)
            {
                var y = mapY * GameMap.TileTextureSize;
                Raylib.DrawLine(0, y, _gameWorldTexture.RenderTexture.texture.width, y, Colors.GridLine);
            }

            // Draw mouse debug elements
            if (_gameWorldTexture.DestinationRectangle.PointInsideRectangle(GameMap.GetMapMousePosition()))
            {
                var mapMouseTile = GameMap.GetMapMouseTile();

                // Draw box around tile that the mouse cursor is inside
                Raylib.DrawRectangleLines(
                    mapMouseTile.Item1 * GameMap.TileTextureSize,
                    mapMouseTile.Item2 * GameMap.TileTextureSize,
                    GameMap.TileTextureSize,
                    GameMap.TileTextureSize,
                    Raylib.RED
                );

                // Draw a dot that marks the mouse cursors position on the map
                Raylib.DrawCircleV(GameMap.GetMapMousePosition(), 2.0f, Raylib.RED);
            }
        }

        // Stop rendering to texture
        Raylib.EndMode2D();
        Raylib.EndTextureMode();

        // Draw texture to screen
        _gameWorldTexture.Draw(true);
    }

    public void Update()
    {
        // Update objects that rely on in game time passage
        UpdateGameObjects();

        // Update mouse structure position to match mouse cursor position 
        if (MouseStructure != null) MouseStructure.MapPosition = GameMap.GetMapMouseTile();
    }

    /// <summary>
    ///     Adds an entity to the entity dictionary
    /// </summary>
    /// <param name="entity"> Entity that we want to add to the dictionary </param>
    /// <exception cref="ArgumentNullException"> If entity is null </exception>
    public void AddEntity(EntityBase entity)
    {
        var entityType = entity.GetType();
        if (entityType == null)
            throw new ArgumentNullException(nameof(entityType));
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var list = _entities.GetValueOrDefault(entityType) ?? new List<EntityBase>();

        list.Add(entity);
        _entities[entityType] = list;
    }

    /// <summary>
    ///     Slows down passage of in game time
    /// </summary>
    public void DecreaseGameSpeed()
    {
        if (GameSpeed == GameSpeed.Slowest) return;

        GameSpeed = GameSpeed.GetPrevious();
    }

    /// <summary>
    ///     Gets a list of all entities
    /// </summary>
    /// <param name="reverse"> Reverses the list before returning it </param>
    /// <returns> List containing all active entities </returns>
    public List<EntityBase> GetAllEntities(bool reverse = false)
    {
        var allEntities = new List<EntityBase>();
        foreach (var entityList in _entities.Values) allEntities.AddRange(entityList);

        if (reverse) allEntities.Reverse();

        return allEntities;
    }

    /// <summary>
    ///     Gets a list of entities with the given type
    /// </summary>
    /// <typeparam name="T"> Entity type that we are getting a list of </typeparam>
    /// <returns> List of entity objects of the given type </returns>
    public List<EntityBase> GetEntityList<T>() where T : class
    {
        var entityType = typeof(T);
        return _entities.GetValueOrDefault(entityType) ?? new List<EntityBase>();
    }

    /// <summary>
    ///     Gets a list of entities with the given base type
    /// </summary>
    /// <typeparam name="T"> Entity base type that we are getting a list of </typeparam>
    /// <returns> List of entity objects of the given base type </returns>
    public List<EntityBase> GetEntityBaseTypeList<T>() where T : class
    {
        var entityList = new List<EntityBase>();
        foreach (var entityType in _entities.Keys.Where(entityType => entityType.BaseType == typeof(T)))
            entityList.AddRange(_entities[entityType]);

        return entityList;
    }

    /// <summary>
    ///     Speeds up passage of in game time
    /// </summary>
    public void IncreaseGameSpeed()
    {
        if (GameSpeed == GameSpeed.Fastest) return;

        GameSpeed = GameSpeed.GetNext();
    }

    /// <summary>
    ///     Attempts to place the selected structure on the map at the mouse cursors position
    /// </summary>
    public void PlaceMouseStructure()
    {
        if (MouseStructure == null) return;
        if (!MouseStructure.ValidPlacement()) return;

        // If all checks pass, add the structure to the main list and remove it from the mouse
        AddEntity(MouseStructure);
        Raylib.PlaySound(Assets.Sounds["click"]); // TODO: Replace this with a 'construction' sound
        MouseStructure = null;
    }

    /// <summary>
    ///     Resets the in game date to the start date
    /// </summary>
    public void ResetDateTime()
    {
        CurrentDateTime = StartDateTime;
    }

    /// <summary>
    ///     Sets a structure to the mouse cursor
    /// </summary>
    /// <param name="structure"> Structure that is being set to the mouse cursor </param>
    public void SetMouseStructure(StructureBase structure)
    {
        UnsetSelectedEntity();
        MouseStructure = structure;
    }

    /// <summary>
    ///     Sets the selected entity
    /// </summary>
    /// <param name="entity"> Entity that has been selected </param>
    public void SetSelectedEntity(EntityBase entity)
    {
        SelectedEntity = entity;
        Raylib.PlaySound(Assets.Sounds["click"]); // TODO: Replace this with a unique sound for each entity type
    }

    /// <summary>
    ///     Toggles pausing the game
    /// </summary>
    public void ToggleGamePaused()
    {
        GamePaused = !GamePaused;
    }

    /// <summary>
    ///     Unsets the selected entity by setting it to null
    /// </summary>
    public void UnsetSelectedEntity()
    {
        SelectedEntity = null;
    }

    /// <summary>
    ///     Updates game objects that rely on the in game passage of time
    /// </summary>
    private void UpdateGameObjects()
    {
        _updateTimer += Raylib.GetFrameTime();
        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            _updateTimer = 0;

            // If game is paused don't update anything
            if (GamePaused) return;

            // Advance in game clock
            CurrentDateTime = CurrentDateTime.AddHours(1);

            // Update game map
            GameMap.Update();

            // Update entities
            foreach (var entityList in _entities.Values)
            foreach (var entity in entityList)
                entity.Update();
        }
    }
}