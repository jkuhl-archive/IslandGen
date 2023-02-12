using System.Numerics;
using Raylib_CsLo;

namespace IslandGen.Data.Textures;

public class AnimatedTexture
{
    private readonly int _frameDelay;
    private readonly int _frameWidth;
    private readonly Texture _texture;
    private readonly int _totalFrames;
    private int _currentFrame;

    private int _frameCounter;
    private Rectangle _textureRectangle;

    /// <summary>
    ///     Constructor for the AnimatedTexture
    /// </summary>
    /// <param name="texture"> Texture containing animation frames stored horizontally </param>
    /// <param name="targetFrameRate"> Framerate the game should be running at </param>
    /// <param name="frameWidth"> Width of animation frames </param>
    public AnimatedTexture(Texture texture, int targetFrameRate, int frameWidth = 16)
    {
        _texture = texture;
        _frameWidth = frameWidth;
        _totalFrames = texture.width / _frameWidth;
        _frameDelay = (int)Math.Round(targetFrameRate / (float)_totalFrames);

        _currentFrame = 0;
        _textureRectangle = new Rectangle(0, 0, _frameWidth, _texture.height);
    }

    public void Draw(Vector2 position)
    {
        Raylib.DrawTextureRec(_texture, _textureRectangle, position, Raylib.WHITE);
    }

    public void Update()
    {
        _frameCounter++;

        if (_frameCounter >= _frameDelay)
        {
            _frameCounter = 0;
            _currentFrame++;

            if (_currentFrame > _totalFrames - 1) _currentFrame = 0;

            _textureRectangle.X = _currentFrame * _frameWidth;
        }
    }
}