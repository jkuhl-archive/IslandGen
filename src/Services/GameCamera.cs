using Raylib_CsLo;

namespace IslandGen.Services;

public class GameCamera
{
    private const float MinZoom = 0.4f;
    private const float MaxZoom = 1.0f;
    private const float ZoomIncrement = 0.1f;
    private const float PanIncrement = 100.0f;

    public Camera2D Camera;

    /// <summary>
    ///     Service that manages the game's camera
    /// </summary>
    public GameCamera()
    {
        Camera = new Camera2D
        {
            zoom = MinZoom
        };
    }

    /// <summary>
    ///     Pans the camera up
    /// </summary>
    public void PanUp()
    {
        var limit = -PanIncrement * 2;
        if (Camera.target.Y - PanIncrement >= limit) Camera.target.Y -= PanIncrement;
    }

    /// <summary>
    ///     Pans the camera down
    /// </summary>
    public void PanDown()
    {
        // TODO: Add auto calculated limit
        Camera.target.Y += PanIncrement;
    }

    /// <summary>
    ///     Pans the camera left
    /// </summary>
    public void PanLeft()
    {
        var limit = -PanIncrement * 2;
        if (Camera.target.X - PanIncrement >= limit) Camera.target.X -= PanIncrement;
    }

    /// <summary>
    ///     Pans the camera right
    /// </summary>
    public void PanRight()
    {
        // TODO: Add auto calculated limit
        Camera.target.X += PanIncrement;
    }

    /// <summary>
    ///     Zooms the camera in
    /// </summary>
    public void ZoomIn()
    {
        if (Camera.zoom + ZoomIncrement is >= MinZoom and <= MaxZoom) Camera.zoom += ZoomIncrement;
    }

    /// <summary>
    ///     Zooms the camera out
    /// </summary>
    public void ZoomOut()
    {
        if (Camera.zoom - ZoomIncrement is >= MinZoom and <= MaxZoom) Camera.zoom -= ZoomIncrement;
    }
}