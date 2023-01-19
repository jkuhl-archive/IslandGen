using System.Numerics;
using IslandGen.Extensions;
using Raylib_CsLo;

namespace IslandGen.Data;

public class RenderTexturePro
{
    public readonly RenderTexture RenderTexture;
    public float Scale;
    public Rectangle SourceRectangle;
    public Rectangle DestinationRectangle;

    /// <summary>
    ///     Constructor for RenderTexturePro
    /// </summary>
    /// <param name="textureSize"> Vector2 that contains the width and height of the texture </param>
    public RenderTexturePro(Vector2 textureSize)
    {
        RenderTexture = Raylib.LoadRenderTexture(textureSize.X_int(), textureSize.Y_int());
        Scale = 1.0f;
        SourceRectangle =
            new Rectangle(0, 0, RenderTexture.texture.width,
                -RenderTexture.texture.height); // The source rectangle's height is flipped for OpenGL reasons
        DestinationRectangle = new Rectangle(0, 0, RenderTexture.texture.width, RenderTexture.texture.height);
    }

    /// <summary>
    ///     Draws this texture to the screen, must be called within a draw session
    /// </summary>
    public void Draw()
    {
        Raylib.DrawTexturePro(
            RenderTexture.texture,
            SourceRectangle,
            DestinationRectangle with
            {
                width = DestinationRectangle.width * Scale, height = DestinationRectangle.height * Scale
            },
            Vector2.Zero,
            0.0f,
            Raylib.WHITE);
    }
}