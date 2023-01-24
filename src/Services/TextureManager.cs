using IslandGen.Data;
using Raylib_CsLo;

namespace IslandGen.Services;

public class TextureManager
{
    private const string TexturesDirectory = "assets/textures";
    private const string AnimatedTexturesDirectory = $"{TexturesDirectory}/animated";
    private const string StaticTexturesDirectory = $"{TexturesDirectory}/static";

    public readonly Dictionary<string, AnimatedTexture> AnimatedTextures;
    public readonly Dictionary<string, Texture> Textures;

    /// <summary>
    ///     Constructor for TextureManager
    /// </summary>
    /// <param name="targetFrameRate"> Framerate the game should be running at, used for animations </param>
    public TextureManager(int targetFrameRate)
    {
        var animatedTexturesDir = new DirectoryInfo(AnimatedTexturesDirectory);
        var animatedTextureFiles = animatedTexturesDir.GetFiles();
        AnimatedTextures = animatedTextureFiles.ToDictionary(
            textureFile => Path.ChangeExtension(textureFile.Name, null),
            textureFile => new AnimatedTexture(Raylib.LoadTexture(textureFile.FullName), targetFrameRate));

        var textureDirectory = new DirectoryInfo(StaticTexturesDirectory);
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