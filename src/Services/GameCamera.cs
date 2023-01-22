using Raylib_CsLo;

namespace IslandGen.Services;

public class GameCamera
{
    public Camera2D Camera2D;

    /// <summary>
    ///     Constructor for GameCamera
    /// </summary>
    public GameCamera()
    {
        Camera2D = new Camera2D
        {
            zoom = 0.8f
        };
    }
}