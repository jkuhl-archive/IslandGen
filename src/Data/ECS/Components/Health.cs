namespace IslandGen.Data.ECS.Components;

public class Health : IComponent
{
    /// <summary>
    ///     Component that manages the entity's health
    /// </summary>
    /// <param name="maxHealthPoints"> Maximum number of health points for the entity </param>
    public Health(int maxHealthPoints = 100)
    {
        MaxMaxHealthPoints = maxHealthPoints;
        HealthPoints = MaxMaxHealthPoints;
    }

    public int MaxMaxHealthPoints { get; }
    public int HealthPoints { get; }
}