using IslandGen.Objects.ECS.Components;
using IslandGen.Objects.ECS.Routines;
using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS;

public abstract class EntityBase
{
    [JsonProperty] private readonly Dictionary<Type, IComponent> _components = new();
    [JsonProperty] private readonly Dictionary<Type, IRoutine> _routines = new();
    [JsonProperty] public readonly Guid Id = Guid.NewGuid();
    [JsonIgnore] private IRoutine? _currentRoutine;
    [JsonProperty] private string? _currentRoutineName;
    [JsonIgnore] protected Texture? Texture;
    [JsonProperty] public (int, int) MapPosition { get; set; }
    [JsonProperty] public string? ReadableName { get; set; }
    [JsonIgnore] public (int, int) Size { get; protected init; }
    [JsonIgnore] public Color MiniMapColor { get; protected init; }

    public virtual void Draw()
    {
        if (Texture != null)
            Raylib.DrawTextureV(Texture.Value,
                ServiceManager.GetService<GameLogic>().GameMap.GetTileCoordinates(MapPosition),
                Raylib.WHITE);
    }

    public virtual void Update()
    {
        // Update components
        foreach (var component in _components.Values) component.Update(this);

        // If we have a routine name but the routine is null attempt to get the routine
        if (_currentRoutine == null && _currentRoutineName != null)
            foreach (var routine in _routines.Values.Where(routine => routine.Name == _currentRoutineName))
                SetCurrentRoutine(routine);

        // If current routine is null, find a new routine
        if (_currentRoutine == null)
            switch (_routines.Count)
            {
                case 0:
                    return;
                case 1:
                    SetCurrentRoutine(_routines.Values.ElementAt(0));
                    break;
                default:
                    if (HasRoutine<Build>() && GetRoutine<Build>().CanExecute(this))
                        SetCurrentRoutine(GetRoutine<Build>());
                    else if (HasRoutine<Wander>()) SetCurrentRoutine(GetRoutine<Wander>());
                    break;
            }

        // Execute routine
        _currentRoutine?.Update(this);
    }

    /// <summary>
    ///     Adds a component
    /// </summary>
    /// <typeparam name="T"> Type of the component </typeparam>
    /// <param name="component"> IComponent object </param>
    /// <exception cref="ArgumentNullException"> If component is null </exception>
    protected void AddComponent<T>(T? component)
    {
        var componentType = typeof(T);
        if (componentType == null)
            throw new ArgumentNullException(nameof(componentType));
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        _components.Add(componentType, (IComponent)component);
    }

    /// <summary>
    ///     Adds a routine
    /// </summary>
    /// <typeparam name="T"> Type of the routine </typeparam>
    /// <param name="routine"> IRoutine object </param>
    /// <exception cref="ArgumentNullException"> If routine is null </exception>
    protected void AddRoutine<T>(T? routine)
    {
        var routineType = typeof(T);
        if (routineType == null)
            throw new ArgumentNullException(nameof(routineType));
        if (routine == null)
            throw new ArgumentNullException(nameof(routine));

        _routines.Add(routineType, (IRoutine)routine);
    }

    /// <summary>
    ///     Gets a component of the specified type
    /// </summary>
    /// <typeparam name="T"> Type of the component </typeparam>
    /// <returns> Component of the specified type or null </returns>
    /// <exception cref="ArgumentNullException"> If component is null </exception>
    public T GetComponent<T>() where T : class
    {
        var componentType = typeof(T);
        if (componentType == null)
            throw new ArgumentNullException(nameof(componentType));

        if (_components.TryGetValue(componentType, out var component))
            if (component == null)
                throw new NullReferenceException(nameof(componentType));

        return (T)component!;
    }

    /// <summary>
    ///     Gets a routine of the specified type
    /// </summary>
    /// <typeparam name="T"> Type of the routine </typeparam>
    /// <returns> Routine of the specified type or null </returns>
    /// <exception cref="ArgumentNullException"> If routine is null </exception>
    public T GetRoutine<T>() where T : class
    {
        var routineType = typeof(T);
        if (routineType == null)
            throw new ArgumentNullException(nameof(routineType));

        if (_routines.TryGetValue(routineType, out var routine))
            if (routine == null)
                throw new NullReferenceException(nameof(routineType));

        return (T)routine!;
    }

    /// <summary>
    ///     Checks if the entity has the component of the given type
    /// </summary>
    /// <typeparam name="T"> Type of the component </typeparam>
    /// <returns> True if the entity has the component, false if not </returns>
    /// <exception cref="ArgumentNullException"> If component is null </exception>
    public bool HasComponent<T>()
    {
        var componentType = typeof(T);
        if (componentType == null)
            throw new ArgumentNullException(nameof(componentType));

        return _components.Keys.Contains(componentType);
    }

    /// <summary>
    ///     Checks if the entity has the routine of the given type
    /// </summary>
    /// <typeparam name="T"> Type of the routine </typeparam>
    /// <returns> True if the entity has the routine, false if not </returns>
    /// <exception cref="ArgumentNullException"> If routine is null </exception>
    public bool HasRoutine<T>()
    {
        var routineType = typeof(T);
        if (routineType == null)
            throw new ArgumentNullException(nameof(routineType));

        return _routines.Keys.Contains(routineType);
    }

    /// <summary>
    ///     Returns a string containing basic information about the entity
    /// </summary>
    /// <returns> String containing entity info </returns>
    public string GetInfoString()
    {
        return $"Type: {GetType().Name}\n" +
               $"Name: {ReadableName}\n" +
               $"Map Position: {MapPosition}\n" +
               $"Size: {Size}\n" +
               $"Status: {GetRoutineStatus()}\n\n" +
               string.Join("\n",
                   _components.Values.Select(component => $"{component.GetType().Name}: {component.GetInfoString()}")
                       .ToList());
    }

    /// <summary>
    ///     Gets the space the entity occupying on the game map as a rectangle
    /// </summary>
    /// <returns> Rectangle containing the space the entity is occupies on the game map </returns>
    public Rectangle GetMapSpaceRectangle()
    {
        var gameMap = ServiceManager.GetService<GameLogic>().GameMap;
        var position = gameMap.GetTileCoordinates(MapPosition);

        return new Rectangle(
            position.X,
            position.Y,
            Size.Item1 * gameMap.GetTileTextureSize(),
            Size.Item2 * gameMap.GetTileTextureSize()
        );
    }

    /// <summary>
    ///     Gets the tiles on the game map that this structure is occupying
    /// </summary>
    /// <returns> List to tuples containing the X and Y positions of all occupied map tiles </returns>
    public List<(int, int)> GetOccupiedTiles()
    {
        var tiles = new List<(int, int)>();
        for (var mapX = MapPosition.Item1; mapX < MapPosition.Item1 + Size.Item1; mapX++)
        for (var mapY = MapPosition.Item2; mapY < MapPosition.Item2 + Size.Item2; mapY++)
            tiles.Add((mapX, mapY));

        return tiles;
    }

    /// <summary>
    ///     Gets the status of the current routine
    /// </summary>
    /// <returns> String containing the status of the current routine, or 'Inactive' if routine is null </returns>
    public string GetRoutineStatus()
    {
        return _currentRoutine == null ? "Inactive" : _currentRoutine.GetStatus();
    }

    /// <summary>
    ///     Moves the entity towards the given position on the game map
    /// </summary>
    /// <param name="newPosition"> Position the entity should move towards </param>
    public void MoveTowards((int, int) newPosition)
    {
        var movementDistance = GetComponent<MovementSpeed>().Speed;

        if (MapPosition.Item1 < newPosition.Item1)
        {
            MapPosition = MapPosition with { Item1 = MapPosition.Item1 + movementDistance };
            return;
        }

        if (MapPosition.Item1 > newPosition.Item1)
        {
            MapPosition = MapPosition with { Item1 = MapPosition.Item1 - movementDistance };
            return;
        }

        if (MapPosition.Item2 < newPosition.Item2)
        {
            MapPosition = MapPosition with { Item2 = MapPosition.Item2 + movementDistance };
            return;
        }

        if (MapPosition.Item2 > newPosition.Item2)
            MapPosition = MapPosition with { Item2 = MapPosition.Item2 - movementDistance };
    }

    /// <summary>
    ///     Unsets the current routine by setting it to null
    /// </summary>
    public void UnsetCurrentRoutine()
    {
        _currentRoutine = null;
        _currentRoutineName = null;
    }

    /// <summary>
    ///     Sets the current routine to the given routine
    /// </summary>
    /// <param name="routine"> IRoutine object that we want to set the current routine to </param>
    private void SetCurrentRoutine(IRoutine routine)
    {
        _currentRoutine = routine;
        _currentRoutineName = routine.Name;
    }
}