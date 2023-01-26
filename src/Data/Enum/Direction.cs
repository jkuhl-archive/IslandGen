namespace IslandGen.Data.Enum;

/// <summary>
///     Cardinal directions
/// </summary>
public enum Direction
{
    North,
    South,
    East,
    West,
    None
}

public static class DirectionExtensions
{
    /// <summary>
    ///     Returns the opposite of the current direction
    /// </summary>
    /// <param name="direction"> Current direction </param>
    /// <returns> Opposite of current direction </returns>
    public static Direction GetOppositeDirection(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            Direction.None => Direction.None,
            _ => Direction.None
        };
    }
}