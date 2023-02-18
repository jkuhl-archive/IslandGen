using IslandGen.Objects.ECS.Components;
using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS;

public abstract class EntityBase
{
    [JsonProperty] private readonly Dictionary<Type, IComponent> _components = new();
    [JsonProperty] private readonly Guid _id = Guid.NewGuid();
    [JsonIgnore] protected Texture? Texture;
    [JsonProperty] public (int, int) MapPosition { get; set; }
    [JsonProperty] public string? ReadableName { get; set; }
    [JsonIgnore] public (int, int) Size { get; protected init; }
    [JsonIgnore] public Color MiniMapColor { get; protected init; }

    public void Draw()
    {
        if (Texture != null)
            Raylib.DrawTextureV(Texture.Value,
                ServiceManager.GetService<GameLogic>().GameMap.GetTileCoordinates(MapPosition),
                Raylib.WHITE);
    }

    public void Update()
    {
        foreach (var component in _components.Values) component.Update(this);
    }

    /// <summary>
    ///     Adds a component
    /// </summary>
    /// <typeparam name="T"> Type of the component </typeparam>
    /// <param name="component"> Component object </param>
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
    ///     Gets a component of the specified type
    /// </summary>
    /// <typeparam name="T"> Type of the component </typeparam>
    /// <returns> Component of the specified type or null </returns>
    public T GetComponent<T>() where T : class
    {
        var requestedType = typeof(T);
        if (requestedType == null)
            throw new ArgumentNullException(nameof(requestedType));

        if (_components.TryGetValue(requestedType, out var component))
            if (component == null)
                throw new NullReferenceException(nameof(requestedType));

        return (T)component!;
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
               $"Size: {Size}\n\n" +
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
    ///     Moves the entity towards the given position on the game map
    /// </summary>
    /// <param name="newPosition"> Position the entity should move towards </param>
    public void MoveTo((int, int) newPosition)
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
}