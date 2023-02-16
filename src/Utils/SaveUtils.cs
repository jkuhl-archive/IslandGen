using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Extensions;
using IslandGen.Objects.ECS.Entities.Creatures;
using IslandGen.Objects.ECS.Entities.Structures;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Utils;

public static class SaveUtils
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All
    };

    /// <summary>
    ///     Loads the saved game
    /// </summary>
    public static void LoadGame()
    {
        if (!File.Exists(Paths.GameLogicFile)) return;
        if (!File.Exists(Paths.GameMapFile)) return;

        var gameLogic = JsonConvert.DeserializeObject<GameLogic>(File.ReadAllText(Paths.GameLogicFile), JsonSettings);
        if (gameLogic == null) return;
        var gameMap = JsonConvert.DeserializeObject<GameMap>(File.ReadAllText(Paths.GameMapFile), JsonSettings);
        if (gameMap == null) return;

        ServiceManager.ReplaceService(gameLogic);
        ServiceManager.ReplaceService(gameMap);

        ServiceManager.GetService<StateManager>().SetGameState(GameState.InGame);
    }

    /// <summary>
    ///     Loads saved game settings
    /// </summary>
    /// <returns> Loaded GameSettings object or new GameSettings object if unable to load </returns>
    public static GameSettings LoadSettings()
    {
        if (!File.Exists(Paths.GameSettingsFile)) return new GameSettings();

        var gameSettings =
            JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(Paths.GameSettingsFile), JsonSettings);
        return gameSettings ?? new GameSettings();
    }

    /// <summary>
    ///     Initializes a new game
    ///     TODO: Move this to MainMenuUi once we no longer want to generate new island in game
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

        ServiceManager.GetService<StateManager>().SetGameState(GameState.InGame);
    }

    /// <summary>
    ///     Saves the game
    /// </summary>
    public static void SaveGame()
    {
        if (!Directory.Exists(Paths.GameSavesDirectory)) Directory.CreateDirectory(Paths.GameSavesDirectory);

        File.WriteAllText(
            Paths.GameLogicFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameLogic>(),
                JsonSettings));
        File.WriteAllText(
            Paths.GameMapFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(),
                JsonSettings));
    }

    /// <summary>
    ///     Saves the game's settings
    /// </summary>
    public static void SaveGameSettings()
    {
        if (!Directory.Exists(Paths.GameSavesDirectory)) Directory.CreateDirectory(Paths.GameSavesDirectory);

        File.WriteAllText(
            Paths.GameSettingsFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameSettings>(),
                JsonSettings));
    }
}