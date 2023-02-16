using System.Reflection;

namespace IslandGen.Data;

public static class Paths
{
    public static readonly string GameDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string AssetsDirectory = Path.Join(GameDirectory, "assets");

    // Game save file paths
    public static readonly string GameSavesDirectory = Path.Join(GameDirectory, "saves");
    public static readonly string GameLogicFile = Path.Join(GameSavesDirectory, "game_logic.json");
    public static readonly string GameMapFile = Path.Join(GameSavesDirectory, "game_map.json");
    public static readonly string GameSettingsFile = Path.Join(GameSavesDirectory, "game_settings.json");

    // Texture paths
    public static readonly string TexturesDirectory = Path.Join(AssetsDirectory, "textures");
    public static readonly string AnimatedTexturesDirectory = Path.Join(TexturesDirectory, "animated");
    public static readonly string StaticTexturesDirectory = Path.Join(TexturesDirectory, "static");
}