using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Utilities;

public static class SaveUtils
{
    private const string SaveFile = "save.json";

    /// <summary>
    ///     Loads the saved map
    /// </summary>
    public static bool LoadMap()
    {
        if (!File.Exists(SaveFile)) return false;

        var mapJson = File.ReadAllText(SaveFile);
        var gameMap = JsonConvert.DeserializeObject<GameMap>(mapJson);
        if (gameMap == null) return false;

        ServiceManager.ReplaceService(JsonConvert.DeserializeObject<GameMap>(mapJson));
        return true;
    }

    /// <summary>
    ///     Saves the current map
    /// </summary>
    public static void SaveMap()
    {
        var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        var mapJson = JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(), jsonSettings);
        File.WriteAllText(SaveFile, mapJson);
    }
}