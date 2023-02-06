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
    /// <param name="x"> X coordinate </param>
    /// <param name="y"> Y coordinate </param>
    /// <returns> True if coordinate is inside rectangle, false if not </returns>
    public static bool PointInsideRectangle(this Rectangle rectangle, int x, int y)
    {
        var start = rectangle.Start();
        var end = rectangle.End();

        return x >= start.X && x <= end.X && y >= start.Y && y <= end.Y;
    }

    /// <summary>
    ///     Checks if the given coordinate exists within the confines of the rectangle
    /// </summary>
    /// <param name="rectangle"> Rectangle we are checking </param>
    /// <param name="position"> Vector2 containing the X and Y coordinates </param>
    /// <returns> True if coordinate is inside rectangle, false if not </returns>
    public static bool PointInsideRectangle(this Rectangle rectangle, Vector2 position)
    {
        return rectangle.PointInsideRectangle(position.X_int(), position.Y_int());
    }

    /// <summary>
    ///     Returns a string that contains the rectangles values
    /// </summary>
    /// <param name="rectangle"> Rectangle that we want to get a string for </param>
    /// <returns> String containing rectangle's values </returns>
    public static string String(this Rectangle rectangle)
    {
        return $"X: {rectangle.x}, Y: {rectangle.y}, W: {rectangle.width}, H: {rectangle.height}";
    }
}