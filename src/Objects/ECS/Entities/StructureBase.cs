using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities;

public class StructureBase : EntityBase
{
    [JsonIgnore] public bool PlaceableOnWater { get; protected init; }

    public override void Draw()
    {
        if (Texture != null)
        {
            var textureColor = Raylib.WHITE;

            // If structure under construction render it with a transparent color
            if (HasComponent<Construction>() && !GetComponent<Construction>().Complete)
                textureColor = Colors.ConstructionBase;

            // Draw structure texture
            Raylib.DrawTextureV(Texture.Value,
                ServiceManager.GetService<GameLogic>().GameMap.GetTileCoordinates(MapPosition),
                textureColor);

            // If structure under construction draw a progress bar over it
            if (HasComponent<Construction>() && !GetComponent<Construction>().Complete)
            {
                var structureMapSpace = GetMapSpaceRectangle();
                var progressBar = new Rectangle(
                    structureMapSpace.X,
                    structureMapSpace.Y + structureMapSpace.height / 4,
                    structureMapSpace.width * GetComponent<Construction>().GetProgressPercentage(),
                    4);
                Raylib.DrawRectangleRec(progressBar, Raylib.GREEN);
            }
        }
    }

    // Giving this an empty body overrides the EntityBase Update method which does things we don't to do.
    // This will be replaced with structure specific logic at some point.
    public override void Update()
    {
    }

    /// <summary>
    ///     Checks if the current position of the structure is valid
    /// </summary>
    /// <returns> True if structure can be placed here, false if not </returns>
    public virtual bool ValidPlacement()
    {
        var gameLogic = ServiceManager.GetService<GameLogic>();
        var occupiedTiles = GetOccupiedTiles();

        // Check if structure is on water
        if (!PlaceableOnWater)
            if (occupiedTiles.Any(tile => gameLogic.GameMap.GetTileType(tile).IsWater()))
                return false;

        // Check if the structure will overlap with any existing structures
        foreach (var structure in gameLogic.GetEntityBaseTypeList<StructureBase>())
            if (structure.GetOccupiedTiles().Intersect(occupiedTiles).Any())
                return false;

        return true;
    }
}