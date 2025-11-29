
using RpgInventory.Models;

namespace RpgInventory.Strategies;

public interface IPotionCombineStrategy
{
    bool CanCombine(Potion a, Potion b);
    Potion Combine(Potion a, Potion b);
}

public sealed class PotencyCombineStrategy : IPotionCombineStrategy
{
    private readonly int _cap;
    public PotencyCombineStrategy(int cap = 150) => _cap = cap;

    public bool CanCombine(Potion a, Potion b) => a.Name == b.Name && a.Effect == b.Effect;

    public Potion Combine(Potion a, Potion b)
    {
        if (!CanCombine(a, b))
            throw new ArgumentException("Potions are not compatible for combination");
        var potency = Math.Min(_cap, a.Potency + b.Potency);
        return new Potion { Name = a.Name, Weight = Math.Max(a.Weight, b.Weight), Rarity = a.Rarity, Effect = a.Effect, Potency = potency, Quantity = 1 };
    }
}
