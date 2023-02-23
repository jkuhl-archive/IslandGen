using IslandGen.Objects.Textures;
using Raylib_CsLo;

namespace IslandGen.Data;

public static class Assets
{
    public static readonly Dictionary<string, AnimatedTexture> AnimatedTextures = LoadAnimatedTextures();
    public static readonly Dictionary<string, Sound> Sounds = LoadSounds();
    public static readonly Dictionary<string, Texture> Textures = LoadTextures();

    /// <summary>
    ///     Loads animated texture assets
    /// </summary>
    /// <returns> Dictionary containing animated texture assets with file names as keys </returns>
    private static Dictionary<string, AnimatedTexture> LoadAnimatedTextures()
    {
        var animatedTextures = new Dictionary<string, AnimatedTexture>();
        var animatedTexturesDirectory = new DirectoryInfo(Paths.AnimatedTexturesDirectory);
        var directoryList = new List<DirectoryInfo> { animatedTexturesDirectory };

        while (directoryList.Count > 0)
        {
            var currentDirectory = directoryList[0];
            directoryList.AddRange(currentDirectory.GetDirectories());

            foreach (var file in currentDirectory.GetFiles())
            {
                var textureName = Path.ChangeExtension(file.Name, null);
                if (currentDirectory != animatedTexturesDirectory)
                    textureName = $"{currentDirectory.Name}/{textureName}";
                animatedTextures.Add(textureName, new AnimatedTexture(Raylib.LoadTexture(file.FullName)));
            }

            directoryList.Remove(currentDirectory);
        }

        return animatedTextures;
    }

    /// <summary>
    ///     Loads sound assets
    /// </summary>
    /// <returns> Dictionary containing sound assets with file names as keys </returns>
    private static Dictionary<string, Sound> LoadSounds()
    {
        var soundsDirectory = new DirectoryInfo(Paths.SoundsDirectory);
        var soundFileList = soundsDirectory.GetFiles();

        return soundFileList.ToDictionary(soundFile => Path.ChangeExtension(soundFile.Name, null),
            soundFile => Raylib.LoadSound(soundFile.FullName));
    }

    /// <summary>
    ///     Loads texture assets
    /// </summary>
    /// <returns> Dictionary containing texture assets with file names as keys </returns>
    private static Dictionary<string, Texture> LoadTextures()
    {
        var textures = new Dictionary<string, Texture>();
        var textureDirectory = new DirectoryInfo(Paths.StaticTexturesDirectory);
        var directoryList = new List<DirectoryInfo> { textureDirectory };

        while (directoryList.Count > 0)
        {
            var currentDirectory = directoryList[0];
            directoryList.AddRange(currentDirectory.GetDirectories());

            foreach (var file in currentDirectory.GetFiles())
            {
                var textureName = Path.ChangeExtension(file.Name, null);
                if (currentDirectory != textureDirectory) textureName = $"{currentDirectory.Name}/{textureName}";
                textures.Add(textureName, Raylib.LoadTexture(file.FullName));
            }

            directoryList.Remove(currentDirectory);
        }

        return textures;
    }

    public static void Update()
    {
        foreach (var animatedTexture in AnimatedTextures.Values)
            animatedTexture.Update();
    }

    /// <summary>
    ///     Unloads asset data from memory
    /// </summary>
    public static void UnloadAssets()
    {
        foreach (var animatedTexture in AnimatedTextures.Values) animatedTexture.Unload();

        foreach (var texture in Textures.Values) Raylib.UnloadTexture(texture);

        foreach (var sound in Sounds.Values) Raylib.UnloadSound(sound);
    }
}