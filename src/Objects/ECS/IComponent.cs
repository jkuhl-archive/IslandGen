namespace IslandGen.Objects.ECS;

public interface IComponent
{
    // Giving this an empty body means that we don't need to implement this method if the component shouldn't update
    public void Update(EntityBase entity)
    {
    }

    public string GetInfoString();
}