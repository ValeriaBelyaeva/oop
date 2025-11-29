using System.Collections.Generic;
using RpgInventory;

namespace RpgInventory.Models;

public sealed class Armor : Item, IEquippable
{
    public int BaseDefense { get; set; } = 0;
    public Slot Slot { get; init; } = Slot.Chest;
    public bool Equipped { get; private set; } = false;
    public IDurabilityState Durability { get; set; } = new PristineState();

    public Armor() {}
    
    public Armor(string name, double weight, Rarity rarity, int baseDefense, Slot slot = Slot.Chest)
    {
        Name = name;
        Weight = weight;
        Rarity = rarity;
        BaseDefense = baseDefense;
        Slot = slot;
    }

    public int Defense => (int)(BaseDefense * Durability.EffectivenessMultiplier);

    public bool CanEquip() => Durability.CanEquip();
    public void OnEquip() => Equipped = true;
    public void OnUnequip() => Equipped = false;
    public void ApplyWear() => Durability = Durability.NextOnUse();

    public override Dictionary<string, object?> Info() => new()
    {
        ["type"] = "armor",
        ["id"] = Id,
        ["name"] = Name,
        ["rarity"] = Rarity.ToString(),
        ["weight"] = Weight,
        ["slot"] = Slot.ToString(),
        ["defense"] = Defense,
        ["base_defense"] = BaseDefense,
        ["equipped"] = Equipped,
        ["durability"] = Durability.Name
    };
}