using IslandGen.Data.Enum;

namespace IslandGen.Services;

public class StateManager
{
    public GameState GameState;

    /// <summary>
    ///     Service that manages the game's state
    /// </summary>
    /// <param name="gameState"> GameState that should be applied on service start </param>
    public StateManager(GameState gameState = GameState.MainMenu)
    {
        GameState = gameState;
    }
}