using System.Collections.Generic;
using RpgInventory;

namespace RpgInventory.Models;

public abstract class Item
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required double Weight { get; init; }
    public required Rarity Rarity { get; init; }

    public abstract Dictionary<string, object?> Info();
}

public interface IEquippable
{
    Slot Slot { get; }
    bool Equipped { get; }
    bool CanEquip();
    void OnEquip();
    void OnUnequip();
    void ApplyWear();
}

public interface IConsumable
{
    int Quantity { get; }
    bool CanUse();
    UseResult Use();
}

public readonly record struct UseResult(string Effect, int Amount, int Remaining);