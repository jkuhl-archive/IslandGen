using System.Numerics;
using Raylib_CsLo;

namespace IslandGen.Data.ECS;

public interface IEntity
{
    public Guid Id { get; }
    public List<IComponent> Components { get; }
    public string ReadableName { get; }
    public Vector2 Position { get; }
    public bool Selectable { get; }
    public Texture? Texture { get; }

    public void Draw()
    {
        if (Texture != null) Raylib.DrawTextureV(Texture.Value, Position, Raylib.WHITE);
    }

    public void Update();
}