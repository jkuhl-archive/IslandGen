using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects;

public class GameCamera
{
    private const float DefaultZoom = 1.0f;
    private const float MinZoom = 0.4f;
    private const float MaxZoom = 2.0f;
    private const float ZoomAmount = 0.1f;
    private const float DefaultPanAmount = 100.0f;

    [JsonProperty] public Camera2D Camera = new() { zoom = DefaultZoom };

    /// <summary>
    ///     Pans the camera up
    ///     <param name="amount"> Distance the camera should be panned </param>
    /// </summary>
    public void PanUp(float amount = DefaultPanAmount)
    {
        const int panLimit = 0;

        Camera.target.Y -= amount;
        if (Camera.target.Y < panLimit) Camera.target.Y = panLimit;
    }

    /// <summary>
    ///     Pans the camera down
    ///     <param name="amount"> Distance the camera should be panned </param>
    /// </summary>
    public void PanDown(float amount = DefaultPanAmount)
    {
        var panLimit = ServiceManager.GetService<GameLogic>().GameMap.GetCameraPanLimits().Item2;

        Camera.target.Y += amount;
        if (Camera.target.Y > panLimit) Camera.target.Y = panLimit;
    }

    /// <summary>
    ///     Pans the camera left
    ///     <param name="amount"> Distance the camera should be panned </param>
    /// </summary>
    public void PanLeft(float amount = DefaultPanAmount)
    {
        const int panLimit = 0;

        Camera.target.X -= amount;
        if (Camera.target.X < panLimit) Camera.target.X = panLimit;
    }

    /// <summary>
    ///     Pans the camera right
    ///     <param name="amount"> Distance the camera should be panned </param>
    /// </summary>
    public void PanRight(float amount = DefaultPanAmount)
    {
        var panLimit = ServiceManager.GetService<GameLogic>().GameMap.GetCameraPanLimits().Item1;

        Camera.target.X += amount;
        if (Camera.target.X > panLimit) Camera.target.X = panLimit;
    }

    /// <summary>
    ///     Zooms the camera in
    /// </summary>
    public void ZoomIn()
    {
        Camera.zoom = (float)Math.Round(Camera.zoom + ZoomAmount, 2);
        if (Camera.zoom > MaxZoom) Camera.zoom = MaxZoom;
    }

    /// <summary>
    ///     Zooms the camera out
    /// </summary>
    public void ZoomOut()
    {
        var previousZoom = Camera.zoom;

        Camera.zoom = (float)Math.Round(Camera.zoom - ZoomAmount, 2);
        if (Camera.zoom < MinZoom) Camera.zoom = MinZoom;

        // Adjust camera target to fit in pan limits, this prevents zooming out into the space outside the game map
        if (Math.Abs(Camera.zoom - previousZoom) > 0)
        {
            var panLimits = ServiceManager.GetService<GameLogic>().GameMap.GetCameraPanLimits();

            if (Camera.target.X > panLimits.Item1) Camera.target.X = panLimits.Item1;
            if (Camera.target.Y > panLimits.Item2) Camera.target.Y = panLimits.Item2;
        }
    }
}