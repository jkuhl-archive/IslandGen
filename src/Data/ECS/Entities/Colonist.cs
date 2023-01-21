using System.Numerics;
using IslandGen.Data.ECS.Components;
using Raylib_CsLo;

namespace IslandGen.Data.ECS.Entities;

public class Colonist : IEntity
{
    /// <summary>
    ///     Constructor for Colonist
    /// </summary>
    /// <param name="readableName"> Colonist's name </param>
    /// <param name="position"> Colonist's starting position </param>
    /// <param name="texture"> Colonist's texture </param>
    public Colonist(string readableName, Vector2 position, Texture texture)
    {
        Id = new Guid();
        ReadableName = readableName;
        Position = position;
        Selectable = true;
        Texture = texture;
        Components = new List<IComponent>
        {
            new Health(100),
            new Inventory(10)
        };
    }

    public Guid Id { get; }
    public List<IComponent> Components { get; }
    public string ReadableName { get; }
    public Vector2 Position { get; }
    public bool Selectable { get; }
    public Texture? Texture { get; }

    public void Update()
    {
        foreach (var component in Components) component.Update(this);
    }
}