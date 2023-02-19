using IslandGen.Data;
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
        if (!File.Exists(Paths.GameSaveFile)) return;

        var gameLogic = JsonConvert.DeserializeObject<GameLogic>(File.ReadAllText(Paths.GameSaveFile), JsonSettings);
        if (gameLogic == null) return;

        ServiceManager.ReplaceService(gameLogic);
        ServiceManager.GetService<StateManager>().InGame();
    }

    /// <summary>
    ///     Loads saved game settings
    /// </summary>
    /// <returns> Loaded GameSettings object or new GameSettings object if unable to load </returns>
    public static void LoadSettings()
    {
        var gameSettings = File.Exists(Paths.GameSettingsFile)
            ? JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(Paths.GameSettingsFile), JsonSettings)
            : new GameSettings();

        gameSettings!.ApplySettings();
        ServiceManager.AddService(gameSettings);
    }

    /// <summary>
    ///     Saves the game
    /// </summary>
    public static void SaveGame()
    {
        if (!Directory.Exists(Paths.GameSavesDirectory)) Directory.CreateDirectory(Paths.GameSavesDirectory);

        File.WriteAllText(
            Paths.GameSaveFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameLogic>(),
                JsonSettings));
    }

    /// <summary>
    ///     Saves the game's settings
    /// </summary>
    public static void SaveSettings()
    {
        if (!Directory.Exists(Paths.GameSavesDirectory)) Directory.CreateDirectory(Paths.GameSavesDirectory);

        File.WriteAllText(
            Paths.GameSettingsFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameSettings>(),
                JsonSettings));
    }
}