using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Data;

public class GameCamera
{
    private const float DefaultZoom = 1.0f;
    private const float MinZoom = 0.4f;
    private const float MaxZoom = 2.0f;
    private const float ZoomIncrement = 0.1f;
    private const float PanIncrement = 100.0f;

    [JsonProperty] public Camera2D Camera = new() { zoom = DefaultZoom };

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
        Camera.zoom = (float)Math.Round(Camera.zoom + ZoomIncrement, 2);

        if (Camera.zoom > MaxZoom) Camera.zoom = MaxZoom;
    }

    /// <summary>
    ///     Zooms the camera out
    /// </summary>
    public void ZoomOut()
    {
        Camera.zoom = (float)Math.Round(Camera.zoom - ZoomIncrement, 2);

        if (Camera.zoom < MinZoom) Camera.zoom = MinZoom;
    }
}