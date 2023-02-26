using IslandGen.Data.Enum;

namespace IslandGen.Services;

public class StateManager
{
    public GameState GameState { get; private set; } = GameState.MainMenu;

    /// <summary>
    ///     Switches the GameState to InGame
    /// </summary>
    public void InGame()
    {
        GameState = GameState.InGame;
    }

    /// <summary>
    ///     Switches the GameState to NewGameMenu
    /// </summary>
    public void NewGameMenu()
    {
        ServiceManager.GetService<NewGameMenuUi>().InitializeGameLogic();
        GameState = GameState.NewGameMenu;
    }

    /// <summary>
    ///     Switches the GameState to MainMenu
    /// </summary>
    public void MainMenu()
    {
        ServiceManager.GetService<MainMenuUi>().UpdateLoadGameButton();
        GameState = GameState.MainMenu;
    }
}