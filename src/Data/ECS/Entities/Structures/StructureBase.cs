using Newtonsoft.Json;

namespace IslandGen.Data.ECS.Entities.Structures;

public class StructureBase : EntityBase
{
    [JsonIgnore] public bool PlaceableOnWater { get; protected init; }
}