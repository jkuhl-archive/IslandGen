using IslandGen.Data.ECS.Entities.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IslandGen.Data.ECS;

[JsonConverter(typeof(StructureConverter))]
public class Structure : EntityBase
{
    [JsonIgnore] public bool PlaceableOnWater { get; protected init; }
    public (int, int) Size { get; protected init; }

    /// <summary>
    ///     Gets the tiles on the game map that this structure is occupying
    /// </summary>
    /// <returns> List to tuples containing the X and Y positions of all occupied map tiles </returns>
    public List<(int, int)> GetStructureOccupiedTiles()
    {
        var tiles = new List<(int, int)>();
        for (var mapX = MapPosition.Item1; mapX < MapPosition.Item1 + Size.Item1; mapX++)
        for (var mapY = MapPosition.Item2; mapY < MapPosition.Item2 + Size.Item2; mapY++)
            tiles.Add((mapX, mapY));

        return tiles;
    }
}

public class StructureConverter : CustomCreationConverter<Structure>
{
    /// <summary>
    ///     Handles converting serialized Structures into objects that implement the Structure class
    ///     TODO: Update this to properly determine structure sub class and return it
    ///     TODO: Enabling `Trim unused assemblies` breaks this class for some reason?
    /// </summary>
    /// <param name="objectType"> Type of the object we are trying to convert </param>
    /// <returns> Structure sub class object </returns>
    public override Structure Create(Type objectType)
    {
        return new Shelter();
    }
}