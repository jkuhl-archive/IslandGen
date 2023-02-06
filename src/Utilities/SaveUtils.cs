using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Utilities;

public static class SaveUtils
{
    private const string GameLogicFile = "game_logic.json";
    private const string MapFile = "game_map.json";

    /// <summary>
    ///     Loads the saved map
    /// </summary>
    public static bool LoadMap()
    {
        if (!File.Exists(MapFile)) return false;
        if (!File.Exists(GameLogicFile)) return false;

        var gameMap = JsonConvert.DeserializeObject<GameMap>(File.ReadAllText(MapFile));
        if (gameMap == null) return false;
        var entitiesManager = JsonConvert.DeserializeObject<GameLogic>(File.ReadAllText(GameLogicFile));
        if (entitiesManager == null) return false;

        ServiceManager.ReplaceService(gameMap);
        ServiceManager.ReplaceService(entitiesManager);

        return true;
    }

    /// <summary>
    ///     Saves the current map
    /// </summary>
    public static void SaveMap()
    {
        var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        File.WriteAllText(MapFile, JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(), jsonSettings));
        File.WriteAllText(GameLogicFile,
            JsonConvert.SerializeObject(ServiceManager.GetService<GameLogic>(), jsonSettings));
    }
}