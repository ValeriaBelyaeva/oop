
using RpgInventory.Models;
using RpgInventory.Strategies;

namespace RpgInventory.Services;

public sealed class UpgradeService
{
    private readonly List<IUpgradeStrategy> _strategies;

    public UpgradeService(IEnumerable<IUpgradeStrategy> strategies)
    {
        _strategies = strategies.ToList();
    }

    public bool Upgrade(Item item)
    {
        bool applied = false;
        foreach (var s in _strategies)
        {
            if (s.AppliesTo(item))
            {
                s.Apply(item);
                applied = true;
            }
        }
        return applied;
    }
}
