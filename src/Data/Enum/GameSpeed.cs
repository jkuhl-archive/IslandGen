namespace IslandGen.Data.Enum;

/// <summary>
///     Speed entities in the game should be updated at
/// </summary>
public enum GameSpeed
{
    Slowest,
    Slower,
    Slow,
    Normal,
    Fast,
    Faster,
    Fastest
}

public static class GameSpeedExtensions
{
    /// <summary>
    ///     Returns the game speed multiplier
    /// </summary>
    /// <param name="gameSpeed"> GameSpeed we want to get the multiplier for </param>
    /// <returns> Multiplier used to modify the amount of time required for in game actions to complete </returns>
    public static float GetSpeedMultiplier(this GameSpeed gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeed.Slowest => 8.0f,
            GameSpeed.Slower => 4.0f,
            GameSpeed.Slow => 2.0f,
            GameSpeed.Normal => 1.0f,
            GameSpeed.Fast => 0.5f,
            GameSpeed.Faster => 0.25f,
            GameSpeed.Fastest => 0.125f,
            _ => 1.0f
        };
    }

    /// <summary>
    ///     Returns the next GameSpeed in the enum, cycles through Slowest to Fastest
    /// </summary>
    /// <param name="gameSpeed"> Current GameSpeed </param>
    /// <returns> Next GameSpeed after the current GameSpeed </returns>
    public static GameSpeed GetNext(this GameSpeed gameSpeed)
    {
        return gameSpeed switch
        {
            GameSpeed.Slowest => GameSpeed.Slower,
            GameSpeed.Slower => GameSpeed.Slow,
            GameSpeed.Slow => GameSpeed.Normal,
            GameSpeed.Normal => GameSpeed.Fast,
            GameSpeed.Fast => GameSpeed.Faster,
            GameSpeed.Faster => GameSpeed.Fastest,
            GameSpeed.Fastest => GameSpeed.Slowest,
            _ => GameSpeed.Normal
        };
    }
}