using IslandGen.Data.ECS;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class EntityManager
{
    public readonly List<EntityBase> Entities;

    private float _updateTimer;
    public GameSpeed GameSpeed;

    /// <summary>
    ///     Service that manages the game's entities
    /// </summary>
    public EntityManager()
    {
        Entities = new List<EntityBase>();
        GameSpeed = GameSpeed.Normal;
    }

    /// <summary>
    ///     Constructor for loading a saved EntityManager
    /// </summary>
    /// <param name="entities"> List of entities </param>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    [JsonConstructor]
    private EntityManager(List<EntityBase> entities, GameSpeed gameSpeed)
    {
        Entities = entities;
        GameSpeed = gameSpeed;
    }

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var entity in Entities) entity.Draw();
    }

    public void Update()
    {
        _updateTimer += Raylib.GetFrameTime();

        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            foreach (var entity in Entities) entity.Update();
            _updateTimer = 0;
        }
    }
}