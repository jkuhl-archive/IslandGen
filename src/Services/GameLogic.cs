using IslandGen.Data;
using IslandGen.Data.ECS;
using IslandGen.Data.ECS.Entities.Structures;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    private const int StartYear = 1600;
    private const int StartMonth = 1;
    private const int StartDay = 1;
    [JsonIgnore] public static readonly DateTime StartDateTime = new(StartYear, StartMonth, StartDay);
    [JsonProperty] private readonly Dictionary<Type, List<EntityBase>> _entities = new();
    [JsonProperty] private float _updateTimer;
    [JsonIgnore] public EntityBase? SelectedEntity;
    [JsonIgnore] public StructureBase? MouseStructure { get; private set; }
    [JsonProperty] public DateTime CurrentDateTime { get; private set; } = StartDateTime;
    [JsonProperty] public GameSpeed GameSpeed { get; private set; } = GameSpeed.Normal;
    [JsonProperty] public GameCamera GameCamera { get; private set; } = new();

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var entityList in _entities.Values)
        foreach (var entity in entityList)
            entity.Draw();

        if (SelectedEntity != null)
            Raylib.DrawRectangleLinesEx(SelectedEntity.GetMapSpaceRectangle(), 2, Colors.TransparentGray);
        MouseStructure?.Draw();
    }

    public void Update()
    {
        // Update objects that rely on in game time passage
        _updateTimer += Raylib.GetFrameTime();
        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            _updateTimer = 0;
            CurrentDateTime = CurrentDateTime.AddHours(1);
            ServiceManager.GetService<GameMap>().Update();

            foreach (var entityList in _entities.Values)
            foreach (var entity in entityList)
                entity.Update();
        }

        // Update mouse structure position to match mouse cursor position 
        if (MouseStructure != null)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            MouseStructure.MapPosition = gameMap.GetMapMouseTile();
        }
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
    ///     Toggles current GameSpeed to the next value
    /// </summary>
    public void ChangeSpeed()
    {
        GameSpeed = GameSpeed.GetNext();
    }

    /// <summary>
    ///     Gets a list of all entities
    /// </summary>
    /// <returns> List containing all active entities </returns>
    public List<EntityBase> GetAllEntities()
    {
        var allEntities = new List<EntityBase>();
        foreach (var entityList in _entities.Values) allEntities.AddRange(entityList);

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
    ///     Attempts to place the selected structure on the map at the mouse cursors position
    /// </summary>
    public void PlaceMouseStructure()
    {
        if (MouseStructure == null) return;
        var occupiedTiles = MouseStructure.GetOccupiedTiles();

        // Check if structure is on water
        if (!MouseStructure.PlaceableOnWater)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            foreach (var tile in occupiedTiles)
                if (gameMap.GetTileType(tile).IsWater())
                    return;
        }

        // Check if the structure will overlap with any existing structures
        if (_entities.Keys.Where(entityType => entityType.BaseType == typeof(StructureBase)).Any(entityType =>
                _entities[entityType]
                    .Any(structure => occupiedTiles.Intersect(structure.GetOccupiedTiles()).Any())))
            return;

        // If all checks pass, add the structure to the main list and remove it from the mouse
        AddEntity(MouseStructure);
        MouseStructure = null;
    }

    /// <summary>
    ///     Sets a structure to the mouse cursor
    /// </summary>
    /// <param name="structure"> Structure that is being set to the mouse cursor </param>
    public void SetMouseStructure(StructureBase structure)
    {
        SelectedEntity = null;
        MouseStructure = structure;
    }
}