using IslandGen.Services;

namespace IslandGen.Data.ECS.Components;

public class Wander : IComponent
{
    private readonly int _delay;
    private readonly (int, int) _init = (-1, -1);
    private bool _delayActive;
    private int _delayCounter;
    private (int, int) _destination;

    /// <summary>
    ///     Component that enables the entity to wander around the game map
    /// </summary>
    /// <param name="delay"> Amount of idle between destinations </param>
    public Wander(int delay = 5)
    {
        _delay = delay;
        _delayActive = false;
        _delayCounter = 0;
        _destination = _init;
    }

    public void Update(EntityBase entity)
    {
        if (_destination == _init) _destination = ServiceManager.GetService<GameMap>().GetRandomTile();

        if (_delayActive)
        {
            _delayCounter++;

            if (_delayCounter >= _delay)
                _delayActive = false;
            else
                return;
        }

        if (entity.GetMapPosition() == _destination)
        {
            _delayActive = true;
            _destination = ServiceManager.GetService<GameMap>().GetRandomTile();
            return;
        }

        entity.MoveTo(_destination);
    }
}