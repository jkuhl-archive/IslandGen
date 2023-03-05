using IslandGen.Data;
using IslandGen.Data.Enum;
using IslandGen.Objects.ECS.Entities.Creatures;
using IslandGen.Objects.UI;
using IslandGen.Services;
using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Objects.ECS.Entities;

public class StructureBase : EntityBase
{
    private const int ButtonSize = 25;

    [JsonIgnore] private readonly TextureButton _deconstructButton;

    /// <summary>
    ///     Base class for structures
    /// </summary>
    protected StructureBase()
    {
        _deconstructButton = new TextureButton(
            Assets.Textures["buttons/deconstruct"],
            StartDeconstruction,
            toolTip: new List<string>
            {
                $"Deconstruct {GetType().Name}",
                "Resources are refunded"
            });
    }

    [JsonProperty] public int ConstructionProgress { get; protected set; }
    [JsonIgnore] protected int ConstructionTotalWork { get; init; }
    [JsonIgnore] protected bool PlaceableOnWater { get; init; }
    [JsonProperty] public Guid? WorkerId { get; set; }
    [JsonProperty] public bool Deconstruct { get; private set; }

    public override void Draw()
    {
        if (Texture != null)
        {
            var textureColor = Raylib.WHITE;

            // If structure under construction render it with a transparent color
            if (!ConstructionComplete() || Deconstruct)
                textureColor = Colors.ConstructionBase;

            // If structure is mouse structure and placement isn't valid color it red
            if (ServiceManager.GetService<GameLogic>().MouseStructure == this && !ValidPlacement())
                textureColor = Colors.ConstructionInvalidPlacement;

            // Draw structure texture
            Raylib.DrawTextureV(Texture.Value,
                ServiceManager.GetService<GameLogic>().GameMap.GetTileCoordinates(MapPosition),
                textureColor);

            // If structure under construction draw a progress bar over it
            if (!ConstructionComplete())
            {
                var structureMapSpace = GetMapSpaceRectangle();
                var progressBar = new Rectangle(
                    structureMapSpace.X,
                    structureMapSpace.Y + structureMapSpace.height / 4,
                    structureMapSpace.width * GetConstructionProgress(),
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
    ///     Draws a menu for interacting with the structure if it is selected
    /// </summary>
    public override void DrawSelectedMenu()
    {
        base.DrawSelectedMenu();
        _deconstructButton.Draw();
    }

    /// <summary>
    ///     Updates the contents of the selected entity menu
    /// </summary>
    public override void UpdateSelectedMenu()
    {
        var scalingManager = ServiceManager.GetService<ScalingManager>();
        var scaledSize = ButtonSize * scalingManager.ScaleFactor;

        // Set selected entity info
        SelectedEntityInfo = $"Type: {GetType().Name}\n" +
                             $"Name: {ReadableName}\n" +
                             $"Map Position: {MapPosition}\n" +
                             $"Size: {Size}";

        // Set worker info
        if (WorkerId != null)
        {
            var worker = ServiceManager.GetService<GameLogic>().GetEntityList<Colonist>()
                .Find(i => i.Id == WorkerId);
            SelectedEntityInfo += $"\nWorker: {worker!.ReadableName}";
        }

        // Add construction info
        if (!ConstructionComplete() && !Deconstruct)
            SelectedEntityInfo += $"\nConstruction Progress: {ConstructionProgress}/{ConstructionTotalWork}";
        else if (Deconstruct)
            SelectedEntityInfo += $"\nDeconstruction Progress: {ConstructionProgress}/{ConstructionTotalWork}";

        // Update deconstruct button
        if (Math.Abs(_deconstructButton.Area.width - scaledSize) > 0)
            _deconstructButton.SetArea(new Rectangle(
                SelectedEntityMenuArea.X + SelectedEntityMenuArea.width - SelectedEntityPadding * 4 - scaledSize,
                SelectedEntityMenuArea.Y + SelectedEntityMenuArea.height - SelectedEntityPadding * 4 - scaledSize,
                scaledSize,
                scaledSize
            ));
        _deconstructButton.Update();
    }

    /// <summary>
    ///     'Works' the structure to construct/deconstruct it or produce resources
    /// </summary>
    /// <param name="worker"> Entity working the structure </param>
    public virtual void Work(EntityBase worker)
    {
        if (!ConstructionComplete() || Deconstruct)
        {
            // Construction
            if (!Deconstruct)
            {
                ConstructionProgress++;
            }
            // Deconstruction
            else
            {
                ConstructionProgress--;
                if (ConstructionProgress <= 0)
                {
                    var gameLogic = ServiceManager.GetService<GameLogic>();
                    gameLogic.RemoveEntity(this);
                    foreach (var cost in GetCost()) gameLogic.AddResource(cost.Key, cost.Value);
                    if (gameLogic.SelectedEntity == this) gameLogic.UnsetSelectedEntity();
                }
            }
        }
    }

    /// <summary>
    ///     Checks if construction of this structure has finished
    /// </summary>
    /// <returns> True if construction is complete, false if not </returns>
    public bool ConstructionComplete()
    {
        return ConstructionProgress >= ConstructionTotalWork;
    }

    /// <summary>
    ///     Checks if deconstruction of this structure has finished
    /// </summary>
    /// <returns> True if deconstruction is complete, false if not. Also returns false if not deconstructing </returns>
    public bool DeconstructionComplete()
    {
        if (!Deconstruct) return false;

        return ConstructionProgress <= 0;
    }

    /// <summary>
    ///     Gets construction progress as a float from 0.0 - 1.0
    /// </summary>
    /// <returns> Construction progress as a float </returns>
    private float GetConstructionProgress()
    {
        return (float)ConstructionProgress / ConstructionTotalWork;
    }

    /// <summary>
    ///     Virtual GetCost method for structures to override
    /// </summary>
    /// <returns> Nothing, throws an exception if called </returns>
    public virtual Dictionary<Resource, int> GetCost()
    {
        throw new NotImplementedException();
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

    /// <summary>
    ///     Marks this structure for deconstruction
    /// </summary>
    private void StartDeconstruction()
    {
        Deconstruct = true;
        _deconstructButton.Disabled = true;
    }
}