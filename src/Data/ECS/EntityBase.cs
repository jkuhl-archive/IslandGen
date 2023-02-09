using IslandGen.Data.ECS.Components;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Data.ECS;

public abstract class EntityBase
{
    private readonly Dictionary<Type, IComponent> _components = new();
    private readonly Guid _id = Guid.NewGuid();
    protected Texture? Texture;

    public (int, int) MapPosition { get; set; }
    public string? ReadableName { get; set; }

    public void Draw()
    {
        if (Texture != null)
            Raylib.DrawTextureV(Texture.Value, ServiceManager.GetService<GameMap>().GetTileCoordinates(MapPosition),
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
    ///     Gets the entity's current position on the game map
    /// </summary>
    /// <returns> Tuple containing the entity's X and Y position on the game map </returns>
    public (int, int) GetMapPosition()
    {
        return MapPosition;
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