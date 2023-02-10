using Newtonsoft.Json;

namespace IslandGen.Data.ECS;

public class Structure : EntityBase
{
    [JsonIgnore] public bool PlaceableOnWater { get; protected init; }
}