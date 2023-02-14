using IslandGen.Services;
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
        const int panLimit = 0;

        Camera.target.Y -= PanIncrement;
        if (Camera.target.Y < panLimit) Camera.target.Y = panLimit;
    }

    /// <summary>
    ///     Pans the camera down
    /// </summary>
    public void PanDown()
    {
        var gameMap = ServiceManager.GetService<GameMap>();
        var panLimit = gameMap.GetCameraPanLimits().Item2;

        Camera.target.Y += PanIncrement;
        if (Camera.target.Y > panLimit) Camera.target.Y = panLimit;
    }

    /// <summary>
    ///     Pans the camera left
    /// </summary>
    public void PanLeft()
    {
        const int panLimit = 0;

        Camera.target.X -= PanIncrement;
        if (Camera.target.X < panLimit) Camera.target.X = panLimit;
    }

    /// <summary>
    ///     Pans the camera right
    /// </summary>
    public void PanRight()
    {
        var gameMap = ServiceManager.GetService<GameMap>();
        var panLimit = gameMap.GetCameraPanLimits().Item1;

        Camera.target.X += PanIncrement;
        if (Camera.target.X > panLimit) Camera.target.X = panLimit;
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
        var previousZoom = Camera.zoom;

        Camera.zoom = (float)Math.Round(Camera.zoom - ZoomIncrement, 2);
        if (Camera.zoom < MinZoom) Camera.zoom = MinZoom;

        // Adjust camera target to fit in pan limits, this prevents zooming out into the space outside the game map
        if (Math.Abs(Camera.zoom - previousZoom) > 0)
        {
            var gameMap = ServiceManager.GetService<GameMap>();
            var panLimits = gameMap.GetCameraPanLimits();

            if (Camera.target.X > panLimits.Item1) Camera.target.X = panLimits.Item1;
            if (Camera.target.Y > panLimits.Item2) Camera.target.Y = panLimits.Item2;
        }
    }
}