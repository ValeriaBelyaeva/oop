
using RpgInventory.Models;

namespace RpgInventory.Strategies;

public interface IUpgradeStrategy
{
    bool AppliesTo(Item item);
    void Apply(Item item);
}

public sealed class SharpnessUpgradeStrategy : IUpgradeStrategy
{
    public bool AppliesTo(Item item) => item is Weapon;

    public void Apply(Item item)
    {
        if (item is Weapon w)
        {
            var increment = Math.Max(1, (int)Math.Round(w.BaseDamage * 0.15));
            w.BaseDamage += increment;
        }
    }
}

public sealed class RuneEtchStrategy : IUpgradeStrategy
{
    public bool AppliesTo(Item item) => item is Weapon;

    public void Apply(Item item)
    {
        if (item is Weapon w)
        {
            w.BaseDamage += 5;
            w.Durability = w.Durability.Repair();
        }
    }
}

public sealed class ReinforcePlatesStrategy : IUpgradeStrategy
{
    public bool AppliesTo(Item item) => item is Armor;

    public void Apply(Item item)
    {
        if (item is Armor a)
        {
            var inc = Math.Max(1, (int)Math.Round(a.BaseDefense * 0.20));
            a.BaseDefense += inc;
        }
    }
}
