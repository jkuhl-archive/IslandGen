using System.Numerics;

namespace IslandGen.Extensions;

public static class Vector2Extensions
{
    /// <summary>
    /// Returns the X value in the Vector2 as an int
    /// </summary>
    /// <param name="vector2"> Vector2 we are getting the value from </param>
    /// <returns> X value in the Vector2 as an int </returns>
    public static int X_int(this Vector2 vector2)
    {
        return (int)vector2.X;
    }
    
    /// <summary>
    /// Returns the Y value in the Vector2 as an int
    /// </summary>
    /// <param name="vector2"> Vector2 we are getting the value from </param>
    /// <returns> Y value in the Vector2 as an int </returns>
    public static int Y_int(this Vector2 vector2)
    {
        return (int)vector2.Y;
    }
}