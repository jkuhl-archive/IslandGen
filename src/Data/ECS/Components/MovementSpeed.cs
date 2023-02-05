namespace IslandGen.Data.ECS.Components;

public class MovementSpeed : IComponent
{
    /// <summary>
    ///     Component that manages the entity's movement speed
    /// </summary>
    /// <param name="speed"> Movement speed </param>
    public MovementSpeed(int speed = 1)
    {
        Speed = speed;
    }

    public int Speed { get; }
}