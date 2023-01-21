namespace IslandGen.Data.ECS.Components;

public class Health : IComponent
{
    public Health(int healthPoints)
    {
        HealthPoints = healthPoints;
    }

    public int HealthPoints { get; }
}