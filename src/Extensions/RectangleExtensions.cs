using System.Numerics;
using Raylib_CsLo;

namespace IslandGen.Extensions;

public static class RectangleExtensions
{
    /// <summary>
    ///     Gets the top left coordinate in the rectangle and returns it
    /// </summary>
    /// <param name="rectangle"> Rectangle we are getting coordinates for </param>
    /// <returns> Vector2 that represents the top left corner </returns>
    public static Vector2 Start(this Rectangle rectangle)
    {
        return new Vector2(rectangle.x, rectangle.y);
    }

    /// <summary>
    ///     Gets the bottom right coordinate in the rectangle and returns it
    /// </summary>
    /// <param name="rectangle"> Rectangle we are getting coordinates for </param>
    /// <returns> Vector2 that represents the bottom right corner </returns>
    public static Vector2 End(this Rectangle rectangle)
    {
        return rectangle.Start() + new Vector2(rectangle.width, rectangle.height);
    }

    /// <summary>
    ///     Checks if the given coordinate exists within the confines of the rectangle
    /// </summary>
    /// <param name="rectangle"> Rectangle we are checking </param>
    /// <param name="point"> Vector2 that contains the coordinates we are checking </param>
    /// <returns> True if coordinate is inside rectangle, false if not </returns>
    public static bool PointInsideRectangle(this Rectangle rectangle, Vector2 point)
    {
        var start = rectangle.Start();
        var end = rectangle.End();

        if (point.X >= start.X && point.X <= end.X && point.Y >= start.Y && point.Y <= end.Y) return true;

        return false;
    }
}