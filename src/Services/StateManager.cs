using IslandGen.Data;
using IslandGen.Data.ECS.Entities.Creatures;
using IslandGen.Data.ECS.Entities.Structures;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using Newtonsoft.Json;

namespace IslandGen.Services;

public class StateManager
{
    private const string GameLogicFile = "game_logic.json";
    private const string MapFile = "game_map.json";

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All
    };

    public GameState GameState = GameState.MainMenu;

    /// <summary>
    ///     Loads the saved game
    /// </summary>
    public static void LoadGame()
    {
        if (!File.Exists(GameLogicFile)) return;
        if (!File.Exists(MapFile)) return;

        var gameLogic = JsonConvert.DeserializeObject<GameLogic>(File.ReadAllText(GameLogicFile), JsonSettings);
        if (gameLogic == null) return;
        var gameMap = JsonConvert.DeserializeObject<GameMap>(File.ReadAllText(MapFile), JsonSettings);
        if (gameMap == null) return;

        ServiceManager.ReplaceService(gameLogic);
        ServiceManager.ReplaceService(gameMap);

        ServiceManager.GetService<StateManager>().GameState = GameState.InGame;
    }

    /// <summary>
    ///     Initializes a game
    /// </summary>
    public static void NewGame()
    {
        var gameLogic = new GameLogic();
        ServiceManager.ReplaceService(gameLogic);
        ServiceManager.ReplaceService(new GameMap());

        for (var i = 0; i < 10; i++)
            gameLogic.AddEntity(new Colonist
            {
                MapPosition = ((Wreckage)gameLogic.GetEntityList<Wreckage>()[0]).GetShipExitTile(),
                ReadableName = Datasets.MaleNames.RandomItem()
            });

        ServiceManager.GetService<StateManager>().GameState = GameState.InGame;
    }

    /// <summary>
    ///     Saves the game
    /// </summary>
    public static void SaveGame()
    {
        File.WriteAllText(MapFile, JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(), JsonSettings));
        File.WriteAllText(GameLogicFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameLogic>(), JsonSettings));
    }
}