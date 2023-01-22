using Raylib_CsLo;

namespace IslandGen.Services;

public class TextureManager
{
    private const string TexturesDirectory = "assets/textures";
    public readonly Dictionary<string, Texture> Textures;

    /// <summary>
    ///     Constructor for TextureManager
    /// </summary>
    public TextureManager()
    {
        var textureDirectory = new DirectoryInfo(TexturesDirectory);
        var textureFileList = textureDirectory.GetFiles();
        Textures = textureFileList.ToDictionary(textureFile => Path.ChangeExtension(textureFile.Name, null),
            textureFile => Raylib.LoadTexture(textureFile.FullName));
    }
}