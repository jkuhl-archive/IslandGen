using System.Numerics;
using Raylib_CsLo;

namespace IslandGen.Extensions;

public static class RectangleExtensions
{
    public static Vector2 Start(this Rectangle rectangle)
    {
        return new Vector2(rectangle.x, rectangle.y);
    }
    
    public static Vector2 End(this Rectangle rectangle)
    {
        return rectangle.Start() + new Vector2(rectangle.width, rectangle.height);
    }
    
    public static bool PointInsideRectangle(this Rectangle rectangle, Vector2 point)
    {
        var start = rectangle.Start();
        var end = rectangle.End();

        if (point.X >= start.X && point.X <= end.X && point.Y >= start.Y && point.Y <= end.Y)
        {
            return true;
        }

        return false;
    }
}