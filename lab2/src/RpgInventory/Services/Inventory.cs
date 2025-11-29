using RpgInventory;
using RpgInventory.Models;
using RpgInventory.Repositories;

namespace RpgInventory.Services;

public sealed class Inventory
{
    private readonly IItemRepository _repo;
    private readonly Dictionary<Slot, IEquippable> _equipment = new();
    private readonly double _weightLimit;

    public Inventory(IItemRepository repo, double weightLimit = 100.0)
    {
        _repo = repo;
        _weightLimit = weightLimit;
    }

    public double TotalWeight => _repo.TotalWeight;

    public IReadOnlyList<Item> Items() => _repo.All().ToList();

    public IReadOnlyDictionary<Slot, IEquippable> Equipped() => new Dictionary<Slot, IEquippable>(_equipment);

    public Item Get(Guid id)
    {
        if (_repo.TryGet(id, out var item) && item is not null)
            return item;
        throw new InventoryException($"Item {id} not found");
    }

    public void Add(Item item)
    {
        if (TotalWeight + item.Weight > _weightLimit)
            throw new InventoryException("Weight limit exceeded");
        _repo.Add(item);
    }

    public Item Remove(Guid id)
    {
        var item = Get(id);
        if (item is IEquippable eq && eq.Equipped)
        {
            if (_equipment.TryGetValue(eq.Slot, out var cur) && ReferenceEquals(cur, eq))
            {
                eq.OnUnequip();
                _equipment.Remove(eq.Slot);
            }
        }
        return _repo.Remove(id);
    }

    public void Equip(Guid id)
    {
        var item = Get(id);
        if (item is not IEquippable eq)
            throw new EquipException("Item is not equippable");
        if (!eq.CanEquip())
            throw new EquipException("Item cannot be equipped in its current state");

        if (_equipment.TryGetValue(eq.Slot, out var current))
        {
            if (ReferenceEquals(current, eq))
                return; // already equipped
            current.OnUnequip();
        }

        eq.OnEquip();
        _equipment[eq.Slot] = eq;
    }

    public void Unequip(Slot slot)
    {
        if (_equipment.TryGetValue(slot, out var cur))
        {
            cur.OnUnequip();
            _equipment.Remove(slot);
        }
    }

    public UseResult Use(Guid id)
    {
        var item = Get(id);
        if (item is not IConsumable cons)
            throw new UseException("Item is not consumable");
        if (!cons.CanUse())
            throw new UseException("Item cannot be used (no charges/quantity)");
        var result = cons.Use();
        if (cons.Quantity <= 0)
            _repo.Remove(item.Id);
        return result;
    }

    public InventorySnapshot Snapshot() => new(
        _repo.All().ToList(),
        new Dictionary<Slot, IEquippable>(_equipment),
        TotalWeight,
        _weightLimit
    );
}

public sealed record InventorySnapshot(
    IReadOnlyList<Item> Items,
    IReadOnlyDictionary<Slot, IEquippable> Equipped,
    double TotalWeight,
    double WeightLimit
);
