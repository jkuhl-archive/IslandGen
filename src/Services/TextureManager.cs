using IslandGen.Data;
using IslandGen.Data.Textures;
using Raylib_CsLo;

namespace IslandGen.Services;

public class TextureManager
{
    public readonly Dictionary<string, AnimatedTexture> AnimatedTextures;
    public readonly Dictionary<string, Texture> Textures;

    /// <summary>
    ///     Service that manages loading and serving textures
    /// </summary>
    public TextureManager()
    {
        var animatedTexturesDir = new DirectoryInfo(Paths.AnimatedTexturesDirectory);
        var animatedTextureFiles = animatedTexturesDir.GetFiles();
        AnimatedTextures = animatedTextureFiles.ToDictionary(
            textureFile => Path.ChangeExtension(textureFile.Name, null),
            textureFile => new AnimatedTexture(Raylib.LoadTexture(textureFile.FullName)));

        var textureDirectory = new DirectoryInfo(Paths.StaticTexturesDirectory);
        var textureFileList = textureDirectory.GetFiles();
        Textures = textureFileList.ToDictionary(textureFile => Path.ChangeExtension(textureFile.Name, null),
            textureFile => Raylib.LoadTexture(textureFile.FullName));
    }

    public void Update()
    {
        foreach (var animatedTexture in AnimatedTextures.Values)
            animatedTexture.Update();
    }
}