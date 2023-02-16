namespace IslandGen.Objects.ECS;

public interface IComponent
{
    public void Update(EntityBase entity)
    {
    }

    public string GetInfoString();
}