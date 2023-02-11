using IslandGen.Data.ECS;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    private const int StartYear = 1600;
    private const int StartMonth = 1;
    private const int StartDay = 1;

    [JsonProperty] private readonly Dictionary<Type, List<EntityBase>> _entities;
    [JsonIgnore] public readonly DateTime StartDateTime = new(StartYear, StartMonth, StartDay);

    private Structure? _mouseStructure;
    private float _updateTimer;

    /// <summary>
    ///     Service that manages the game's logic
    /// </summary>
    public GameLogic()
    {
        _entities = new Dictionary<Type, List<EntityBase>>();
        CurrentDateTime = StartDateTime;
        GameSpeed = GameSpeed.Normal;
    }

    /// <summary>
    ///     Constructor for loading a saved GameLogic
    /// </summary>
    /// <param name="entities"> Dictionary containing lists of entities active in game </param>
    /// <param name="currentDateTime"> Current DateTime in game </param>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    [JsonConstructor]
    private GameLogic(Dictionary<Type, List<EntityBase>> entities, DateTime currentDateTime,
        GameSpeed gameSpeed)
    {
        _entities = entities;
        CurrentDateTime = currentDateTime;
        GameSpeed = gameSpeed;
    }

    [JsonProperty] public DateTime CurrentDateTime { get; private set; }
    [JsonProperty] public GameSpeed GameSpeed { get; private set; }

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var entityList in _entities.Values)
        foreach (var entity in entityList)
            entity.Draw();

        _mouseStructure?.Draw();
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
        if (_mouseStructure != null)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            _mouseStructure.MapPosition = gameMap.GetMapMouseTile();
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
        if (_mouseStructure == null) return;
        var occupiedTiles = _mouseStructure.GetOccupiedTiles();

        // Check if structure is on water
        if (!_mouseStructure.PlaceableOnWater)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            foreach (var tile in occupiedTiles)
                if (gameMap.GetTileType(tile).IsWater())
                    return;
        }

        // Check if the structure will overlap with any existing structures
        if (_entities.Keys.Where(entityType => entityType.BaseType == typeof(Structure)).Any(entityType =>
                _entities[entityType]
                    .Any(structure => occupiedTiles.Intersect(structure.GetOccupiedTiles()).Any())))
            return;

        // If all checks pass, add the structure to the main list and remove it from the mouse
        AddEntity(_mouseStructure);
        _mouseStructure = null;
    }

    /// <summary>
    ///     Sets a structure to the mouse cursor
    /// </summary>
    /// <param name="structure"> Structure that is being set to the mouse cursor </param>
    public void SetMouseStructure(Structure structure)
    {
        _mouseStructure = structure;
    }
}