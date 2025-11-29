using RpgInventory;
using RpgInventory.Models;
using RpgInventory.Strategies;

namespace RpgInventory.Services;

public sealed class CombineService
{
    private readonly IPotionCombineStrategy _potionStrategy;

    public CombineService(IPotionCombineStrategy potionStrategy)
    {
        _potionStrategy = potionStrategy;
    }

    public Potion CombinePotions(Potion a, Potion b)
    {
        if (!_potionStrategy.CanCombine(a, b))
            throw new CombineException("These potions cannot be combined");
        return _potionStrategy.Combine(a, b);
    }
}
