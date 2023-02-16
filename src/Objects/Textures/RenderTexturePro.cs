using System.Numerics;
using IslandGen.Extensions;
using IslandGen.Services;
using Raylib_CsLo;

namespace IslandGen.Objects.Textures;

public class RenderTexturePro
{
    public readonly RenderTexture RenderTexture;
    public Rectangle DestinationRectangle;
    public Rectangle SourceRectangle;

    /// <summary>
    ///     Constructor for RenderTexturePro
    /// </summary>
    /// <param name="textureSize"> Vector2 that contains the width and height of the texture </param>
    public RenderTexturePro(Vector2 textureSize)
    {
        RenderTexture = Raylib.LoadRenderTexture(textureSize.X_int(), textureSize.Y_int());

        // The source rectangle's height is flipped for OpenGL reasons
        SourceRectangle = new Rectangle(0, 0, RenderTexture.texture.width, -RenderTexture.texture.height);
        DestinationRectangle = new Rectangle(0, 0, RenderTexture.texture.width, RenderTexture.texture.height);
    }

    /// <summary>
    ///     Draws this texture to the screen, must be called within a draw session
    /// </summary>
    /// <param name="scaled"> If true the destination rectangle will be scaled </param>
    public void Draw(bool scaled = false)
    {
        var destinationRectangle = DestinationRectangle;

        if (scaled)
        {
            var scalingManager = ServiceManager.GetService<ScalingManager>();
            destinationRectangle = destinationRectangle with
            {
                width = DestinationRectangle.width * scalingManager.ScaleFactor,
                height = DestinationRectangle.height * scalingManager.ScaleFactor
            };
        }

        Raylib.DrawTexturePro(
            RenderTexture.texture,
            SourceRectangle,
            destinationRectangle,
            Vector2.Zero,
            0.0f,
            Raylib.WHITE);
    }
}