namespace IslandGen.Objects.ECS;

public interface IRoutine
{
    public string Name { get; }
    public void Update(EntityBase entity);
    public bool CanExecute(EntityBase entity);
    public string GetStatus();
}