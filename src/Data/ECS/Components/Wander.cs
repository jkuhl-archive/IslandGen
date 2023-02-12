using IslandGen.Data.Enum;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Components;

public class Wander : IComponent
{
    [JsonProperty] private readonly int _delay;
    [JsonProperty] private readonly int _distance;
    [JsonProperty] private bool _delayActive;
    [JsonProperty] private int _delayCounter;
    [JsonProperty] private (int, int) _destination;

    /// <summary>
    ///     Component that enables the entity to wander around the game map
    /// </summary>
    /// <param name="delay"> Amount of idle between destinations </param>
    /// <param name="distance"> Max distance the entity can wander at a time </param>
    public Wander(int delay = 3, int distance = 5)
    {
        _delay = delay;
        _distance = distance;

        _delayActive = true;
        _delayCounter = 0;
    }

    public void Update(EntityBase entity)
    {
        if (_delayActive)
        {
            _delayCounter++;

            if (_delayCounter >= _delay)
            {
                _delayActive = false;
                _delayCounter = 0;
                SetDestination(entity.MapPosition);
            }
            else
            {
                return;
            }
        }

        if (entity.MapPosition == _destination)
        {
            _delayActive = true;
            return;
        }

        entity.MoveTo(_destination);
    }

    /// <summary>
    ///     Returns info about entity wandering
    /// </summary>
    /// <returns> Wander status as a string </returns>
    public string GetInfoString()
    {
        return $"Wandering: {!_delayActive}, Destination: {_destination}";
    }

    /// <summary>
    ///     Randomly sets a new destination near the entity's current position
    /// </summary>
    /// <param name="currentMapPosition"> Current position on the game map </param>
    private void SetDestination((int, int) currentMapPosition)
    {
        var gameMap = ServiceManager.GetService<GameMap>();
        var rnd = ServiceManager.GetService<Random>();

        (int, int) destination;
        while (true)
        {
            destination = (currentMapPosition.Item1 + rnd.Next(-_distance, _distance),
                currentMapPosition.Item2 + rnd.Next(-_distance, _distance));
            if (!gameMap.PositionInRange(destination)) continue;
            if (!gameMap.GetTileType(destination).IsWater()) break;
        }

        _destination = destination;
    }
}