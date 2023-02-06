using IslandGen.Data.ECS.Entities;
using IslandGen.Data.Enum;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameLogic
{
    public readonly List<Colonist> Colonists;
    private float _updateTimer;
    public GameSpeed GameSpeed;

    /// <summary>
    ///     Service that manages the game's logic
    /// </summary>
    public GameLogic()
    {
        Colonists = new List<Colonist>();
        GameSpeed = GameSpeed.Normal;
    }

    /// <summary>
    ///     Constructor for loading a saved GameLogic
    /// </summary>
    /// <param name="colonists"> List of colonists </param>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    [JsonConstructor]
    private GameLogic(List<Colonist> colonists, GameSpeed gameSpeed)
    {
        Colonists = colonists;
        GameSpeed = gameSpeed;
    }

    public void Draw()
    {
        // TODO: Add map culling here
        foreach (var colonist in Colonists) colonist.Draw();
    }

    public void Update()
    {
        _updateTimer += Raylib.GetFrameTime();

        if (_updateTimer >= 1 * GameSpeed.GetSpeedMultiplier())
        {
            foreach (var colonist in Colonists) colonist.Update();
            _updateTimer = 0;
        }
    }
}