using IslandGen.Data;
using IslandGen.Data.ECS.Entities;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using Newtonsoft.Json;

namespace IslandGen.Services;

public class StateManager
{
    private const string GameLogicFile = "game_logic.json";
    private const string MapFile = "game_map.json";

    public GameState GameState;

    /// <summary>
    ///     Service that manages the game's state
    /// </summary>
    /// <param name="gameState"> GameState that should be applied on service start </param>
    public StateManager(GameState gameState = GameState.MainMenu)
    {
        GameState = gameState;
    }

    /// <summary>
    ///     Loads the saved game
    /// </summary>
    public static void LoadGame()
    {
        if (!File.Exists(MapFile)) return;
        if (!File.Exists(GameLogicFile)) return;

        var gameMap = JsonConvert.DeserializeObject<GameMap>(File.ReadAllText(MapFile));
        if (gameMap == null) return;
        var entitiesManager = JsonConvert.DeserializeObject<GameLogic>(File.ReadAllText(GameLogicFile));
        if (entitiesManager == null) return;

        ServiceManager.ReplaceService(gameMap);
        ServiceManager.ReplaceService(entitiesManager);

        ServiceManager.GetService<StateManager>().GameState = GameState.InGame;
    }

    /// <summary>
    ///     Initializes a game
    /// </summary>
    public static void NewGame()
    {
        var gameLogic = new GameLogic();
        var gameMap = new GameMap();
        ServiceManager.ReplaceService(gameLogic);
        ServiceManager.ReplaceService(gameMap);

        for (var i = 0; i < 10; i++)
            gameLogic.Colonists.Add(new Colonist(Datasets.MaleNames.RandomItem(), gameMap.GetRandomTile()));

        ServiceManager.GetService<StateManager>().GameState = GameState.InGame;
    }

    /// <summary>
    ///     Saves the game
    /// </summary>
    public static void SaveGame()
    {
        var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        File.WriteAllText(MapFile, JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(), jsonSettings));
        File.WriteAllText(GameLogicFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameLogic>(), jsonSettings));
    }
}