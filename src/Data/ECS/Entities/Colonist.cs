using IslandGen.Data.ECS.Components;
using IslandGen.Services;

namespace IslandGen.Data.ECS.Entities;

public class Colonist : EntityBase
{
    /// <summary>
    ///     Colonist entity
    /// </summary>
    /// <param name="readableName"> Colonist's name </param>
    /// <param name="mapPosition"> Colonist's position on the game map </param>
    public Colonist(string readableName, (int, int) mapPosition) : base(readableName, mapPosition)
    {
        Texture = ServiceManager.GetService<TextureManager>().Textures["colonist"];

        AddComponent(new Health());
        AddComponent(new Inventory());
        AddComponent(new MovementSpeed());
        AddComponent(new Wander());
    }
}