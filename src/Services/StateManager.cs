using IslandGen.Data.Enum;

namespace IslandGen.Services;

public class StateManager
{
    public GameState GameState { get; private set; } = GameState.MainMenu;

    /// <summary>
    ///     Sets the game state
    /// </summary>
    /// <param name="gameState"> GameState that we want to set the game to </param>
    public void SetGameState(GameState gameState)
    {
        GameState = gameState;
    }
}