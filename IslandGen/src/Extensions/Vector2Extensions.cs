using System.Numerics;

namespace IslandGen.Extensions;

public static class Vector2Extensions
{
    public static int X_int(this Vector2 vector2)
    {
        return (int)vector2.X;
    }
    
    public static int Y_int(this Vector2 vector2)
    {
        return (int)vector2.Y;
    }
}