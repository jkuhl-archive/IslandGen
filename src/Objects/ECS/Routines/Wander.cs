using IslandGen.Data.Enum;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Routines;

public class Wander : IRoutine
{
    private const int Delay = 3;
    private const int Distance = 5;

    [JsonProperty] private bool _delayActive = true;
    [JsonProperty] private int _delayCounter;
    [JsonProperty] private (int, int) _destination;
    [JsonIgnore] public string Name => "Wandering";

    public void Update(EntityBase entity)
    {
        if (_delayActive)
        {
            _delayCounter++;

            if (_delayCounter >= Delay)
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
            entity.UnsetCurrentRoutine();
            return;
        }

        entity.MoveTowards(_destination);
    }

    /// <summary>
    ///     Returns true if the entity can wander, which is always true
    /// </summary>
    /// <returns> Always returns true </returns>
    public bool CanExecute(EntityBase entity)
    {
        return true;
    }

    /// <summary>
    ///     Returns wander status
    /// </summary>
    /// <returns> Wander status as a string </returns>
    public string GetStatus()
    {
        return $"Wandering to {_destination}";
    }

    /// <summary>
    ///     Randomly sets a new destination near the entity's current position
    /// </summary>
    /// <param name="currentMapPosition"> Current position on the game map </param>
    private void SetDestination((int, int) currentMapPosition)
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var rnd = ServiceManager.GetService<Random>();

        (int, int) destination;
        while (true)
        {
            destination = (currentMapPosition.Item1 + rnd.Next(-Distance, Distance),
                currentMapPosition.Item2 + rnd.Next(-Distance, Distance));
            if (!gameLogic.GameMap.PositionInRange(destination)) continue;
            if (!gameLogic.GameMap.GetTileType(destination).IsWater()) break;
        }

        _destination = destination;
    }
}