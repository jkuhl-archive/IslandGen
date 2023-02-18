namespace IslandGen.Data;

public static class Paths
{
    public static readonly string GameDirectory = Path.GetDirectoryName(AppContext.BaseDirectory)!;
    public static readonly string AssetsDirectory = Path.Join(GameDirectory, "assets");

    // Game save file paths
    public static readonly string GameSavesDirectory = Path.Join(GameDirectory, "saves");
    public static readonly string GameSaveFile = Path.Join(GameSavesDirectory, "game_save.json");
    public static readonly string GameSettingsFile = Path.Join(GameSavesDirectory, "game_settings.json");

    // Texture paths
    public static readonly string TexturesDirectory = Path.Join(AssetsDirectory, "textures");
    public static readonly string AnimatedTexturesDirectory = Path.Join(TexturesDirectory, "animated");
    public static readonly string StaticTexturesDirectory = Path.Join(TexturesDirectory, "static");

    // Audio paths
    public static readonly string SoundsDirectory = Path.Join(AssetsDirectory, "sounds");
}