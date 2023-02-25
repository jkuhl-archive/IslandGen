using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Components;
using IslandGen.Objects.ECS.Entities;
using IslandGen.Services;
using Newtonsoft.Json;

namespace IslandGen.Objects.ECS.Routines;

public class Build : IRoutine
{
    [JsonIgnore] private StructureBase? _target;
    [JsonProperty] private Guid? _targetId;
    [JsonIgnore] public string Name => "Building";

    public void Update(EntityBase entity)
    {
        // If we have a target ID but the target is null attempt to get the target
        if (_target == null && _targetId != null) _target = GetTarget(_targetId);

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
        var treeList = ServiceManager.GetService<GameLogic>().GetEntityList<Tree>();
        if (treeList.Any(tree => tree.GetOccupiedTiles().Intersect(_target.GetOccupiedTiles()).Any()))
            for (var i = 0; i < treeList.Count; i++)
                if (treeList[i].GetOccupiedTiles().Intersect(_target.GetOccupiedTiles()).Any())
                {
                    treeList.Remove(treeList[i]);
                    ServiceManager.GetService<GameLogic>().AddResource(Resource.TreeTrunk, 1);
                    return;
                }

        // Build the target
        _target.GetComponent<Construction>().Update(entity);

        // If construction is complete, stop building
        if (_target.GetComponent<Construction>().Complete)
        {
            _target = null;
            _targetId = null;
            entity.UnsetCurrentRoutine();
        }
    }

    /// <summary>
    ///     Checks if there is anything that needs to be built
    /// </summary>
    /// <param name="entity"> Entity that will be building </param>
    /// <returns> True if a build target can be found, false if not </returns>
    public bool CanExecute(EntityBase entity)
    {
        if (_target != null) return true;

        _target = GetTarget();
        if (_target != null)
        {
            _targetId = _target.Id;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Returns build status
    /// </summary>
    /// <returns> Build status as a string </returns>
    public string GetStatus()
    {
        return _target == null ? string.Empty : $"Building {_target.ReadableName} at {_target.MapPosition}";
    }

    /// <summary>
    ///     Attempts to find a structure that needs to be built and has not already been started
    /// </summary>
    /// <param name="guid"> If specified attempts to find a structure with the given ID </param>
    /// <returns> StructureBase if a build target could be found, null if not </returns>
    private StructureBase? GetTarget(Guid? guid = null)
    {
        foreach (var structure in ServiceManager.GetService<GameLogic>().GetEntityBaseTypeList<StructureBase>())
        {
            if (!structure.HasComponent<Construction>()) continue;

            if (guid != null && structure.Id == guid) return structure;

            if (structure.GetComponent<Construction>().Complete) continue;
            if (structure.GetComponent<Construction>().Started) continue;

            structure.GetComponent<Construction>().Started = true;
            return structure;
        }

        return null;
    }
}