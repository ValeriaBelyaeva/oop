using System.Collections.Generic;
using RpgInventory;

namespace RpgInventory.Models;

public sealed class Potion : Item, IConsumable
{
    public string Effect { get; set; } = "heal";
    public int Potency { get; set; } = 25;
    public int Quantity { get; set; } = 1;

    public Potion() {}
    
    public Potion(string name, double weight, Rarity rarity, string effect, int potency, int quantity = 1)
    {
        Name = name;
        Weight = weight;
        Rarity = rarity;
        Effect = effect;
        Potency = potency;
        Quantity = quantity;
    }

    public bool CanUse() => Quantity > 0;

    public UseResult Use()
    {
        if (!CanUse())
            throw new InvalidOperationException("Potion cannot be used: quantity is 0");
        Quantity -= 1;
        return new UseResult(Effect, Potency, Quantity);
    }

    public override Dictionary<string, object?> Info() => new()
    {
        ["type"] = "potion",
        ["id"] = Id,
        ["name"] = Name,
        ["rarity"] = Rarity.ToString(),
        ["weight"] = Weight,
        ["effect"] = Effect,
        ["potency"] = Potency,
        ["quantity"] = Quantity
    };
}