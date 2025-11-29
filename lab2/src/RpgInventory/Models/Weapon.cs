using System.Collections.Generic;
using RpgInventory;

namespace RpgInventory.Models;

public sealed class Weapon : Item, IEquippable
{
    public int BaseDamage { get; set; } = 0;
    public Slot Slot { get; init; } = Slot.MainHand;
    public bool Equipped { get; private set; } = false;
    public IDurabilityState Durability { get; set; } = new PristineState();

    public Weapon() {}
    
    public Weapon(string name, double weight, Rarity rarity, int baseDamage, Slot slot = Slot.MainHand)
    {
        Name = name;
        Weight = weight;
        Rarity = rarity;
        BaseDamage = baseDamage;
        Slot = slot;
    }

    public int Damage => (int)(BaseDamage * Durability.EffectivenessMultiplier);

    public bool CanEquip() => Durability.CanEquip();
    public void OnEquip() => Equipped = true;
    public void OnUnequip() => Equipped = false;
    public void ApplyWear() => Durability = Durability.NextOnUse();

    public override Dictionary<string, object?> Info() => new()
    {
        ["type"] = "weapon",
        ["id"] = Id,
        ["name"] = Name,
        ["rarity"] = Rarity.ToString(),
        ["weight"] = Weight,
        ["slot"] = Slot.ToString(),
        ["damage"] = Damage,
        ["base_damage"] = BaseDamage,
        ["equipped"] = Equipped,
        ["durability"] = Durability.Name
    };
}