using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Entities;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Routines;

public class Build : IRoutine
{
    [JsonProperty] private bool _groundCleared;
    [JsonIgnore] private StructureBase? _target;
    [JsonProperty] private Guid? _targetId;
    [JsonProperty] private bool _treesCleared;
    [JsonIgnore] public string Name => "Building";

    public void Update(EntityBase entity)
    {
        // If we have a target ID but the target is null attempt to get the target
        if (_target == null && _targetId != null) _target = GetTarget(entity, _targetId);

        // If target is null stop routine
        if (_target == null)
        {
            entity.UnsetCurrentRoutine();
            return;
        }

        // If entity is not near target move towards it
        if (!_target.GetOccupiedTiles().Contains(entity.MapPosition))
        {
            entity.MoveTowards(_target.MapPosition);
            return;
        }

        // Clear any trees in the build area
        if (!_treesCleared && !_target.Deconstruct)
        {
            var gameLogic = ServiceManager.GetService<GameLogic>();
            var treeList = gameLogic.GetEntityList<Tree>();
            if (treeList.Any(tree => tree.GetOccupiedTiles().Intersect(_target.GetOccupiedTiles()).Any()))
                for (var i = 0; i < treeList.Count; i++)
                    if (treeList[i].GetOccupiedTiles().Intersect(_target.GetOccupiedTiles()).Any())
                    {
                        treeList.Remove(treeList[i]);
                        gameLogic.AddResource(Resource.TreeTrunk, 1);
                        return;
                    }

            _treesCleared = true;
        }

        // Clear ground below build area
        if (!_groundCleared && !_target.Deconstruct)
        {
            var gameLogic = ServiceManager.GetService<GameLogic>();
            foreach (var groundTile in _target.GetOccupiedTiles())
            {
                var tileType = gameLogic.GameMap.GetTileType(groundTile);
                if (tileType.IsGrowable() && tileType != TileType.Dirt)
                {
                    gameLogic.GameMap.SetTileType(groundTile, TileType.Dirt);
                    return;
                }
            }

            _groundCleared = true;
        }

        // Work the target
        _target.Work(entity);

        // If construction is complete, stop building
        if (_target.ConstructionComplete() || _target.DeconstructionComplete()) EndRoutine(entity);
    }

    /// <summary>
    ///     Checks if there is anything that needs to be built
    /// </summary>
    /// <param name="entity"> Entity that will be building </param>
    /// <returns> True if a build target can be found, false if not </returns>
    public bool CanExecute(EntityBase entity)
    {
        if (_target != null) return true;

        _target = GetTarget(entity);
        if (_target != null)
        {
            _targetId = _target.Id;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Exits out of this routine
    /// </summary>
    /// <param name="entity"> Entity that this routine is attached to </param>
    public void EndRoutine(EntityBase entity)
    {
        if (_target != null) _target.WorkerId = null;
        _target = null;
        _targetId = null;
        entity.UnsetCurrentRoutine();
    }

    /// <summary>
    ///     Returns build status
    /// </summary>
    /// <returns> Build status as a string </returns>
    public string GetStatus()
    {
        if (_target != null)
            return _target.Deconstruct switch
            {
                false => $"Building {_target.ReadableName} at {_target.MapPosition}",
                true => $"Deconstructing {_target.ReadableName} at {_target.MapPosition}"
            };

        return string.Empty;
    }

    /// <summary>
    ///     Attempts to find a structure that needs to be built and has not already been started
    /// </summary>
    /// <param name="worker"> Entity that will be working on the structure </param>
    /// <param name="guid"> If specified attempts to find a structure with the given ID </param>
    /// <returns> StructureBase if a build target could be found, null if not </returns>
    private StructureBase? GetTarget(EntityBase worker, Guid? guid = null)
    {
        foreach (var structure in ServiceManager.GetService<GameLogic>().GetEntityBaseTypeList<StructureBase>())
        {
            if (guid != null && structure.Id == guid) return structure;

            if (!structure.Deconstruct && structure.ConstructionComplete()) continue;
            if (structure.WorkerId != null) continue;

            structure.WorkerId = worker.Id;
            return structure;
        }

        return null;
    }
}