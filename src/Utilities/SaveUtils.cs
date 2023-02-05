using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Utilities;

public static class SaveUtils
{
    private const string EntitiesFile = "entities.json";
    private const string MapFile = "map.json";

    /// <summary>
    ///     Loads the saved map
    ///     TODO: Fix entity loading
    /// </summary>
    public static bool LoadMap()
    {
        if (!File.Exists(MapFile)) return false;
        // if (!File.Exists(EntitiesFile)) return false;

        var gameMap = JsonConvert.DeserializeObject<GameMap>(File.ReadAllText(MapFile));
        if (gameMap == null) return false;
        // var entitiesManager = JsonConvert.DeserializeObject<EntityManager>(File.ReadAllText(EntitiesFile));
        // if (entitiesManager == null) return false;

        ServiceManager.ReplaceService(gameMap);
        // ServiceManager.ReplaceService(entitiesManager);

        return true;
    }

    /// <summary>
    ///     Saves the current map
    ///     TODO: Fix entity saving
    /// </summary>
    public static void SaveMap()
    {
        var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        File.WriteAllText(MapFile, JsonConvert.SerializeObject(ServiceManager.GetService<GameMap>(), jsonSettings));
        // File.WriteAllText(EntitiesFile, JsonConvert.SerializeObject(ServiceManager.GetService<EntityManager>(), jsonSettings));
    }
}