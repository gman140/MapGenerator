using MapGenerator.Domain.Interfaces;
using MapGenerator.Domain.Models;
using MapGenerator.Fishing.Interfaces;

namespace MapGenerator.Application.Services;

public class ItemRegistry : IItemRegistry
{
    private readonly IResourceDefinitionProvider _materials;
    private readonly IEquipmentDefinitionProvider _equipment;
    private readonly IConsumableDefinitionProvider _consumables;
    private readonly IFishDefinitionProvider _fish;

    public ItemRegistry(
        IResourceDefinitionProvider materials,
        IEquipmentDefinitionProvider equipment,
        IConsumableDefinitionProvider consumables,
        IFishDefinitionProvider fish)
    {
        _materials   = materials;
        _equipment   = equipment;
        _consumables = consumables;
        _fish        = fish;
    }

    public ResourceDefinition?    FindMaterial(string id)   => _materials.GetById(id);
    public EquipmentDefinition?   FindEquipment(string id)  => _equipment.GetById(id);
    public ConsumableDefinition?  FindConsumable(string id) => _consumables.GetById(id);

    public string? GetName(string id) =>
        (string?)_materials.GetById(id)?.Name
        ?? _equipment.GetById(id)?.Name
        ?? _consumables.GetById(id)?.Name
        ?? _fish.GetById(id)?.Name;
}
