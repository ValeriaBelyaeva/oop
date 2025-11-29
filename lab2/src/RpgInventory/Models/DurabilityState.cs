
namespace RpgInventory.Models;

public interface IDurabilityState
{
    string Name { get; }
    bool CanEquip();
    IDurabilityState NextOnUse();
    IDurabilityState Repair();
    double EffectivenessMultiplier { get; }
}

public sealed class PristineState : IDurabilityState
{
    public string Name => "pristine";
    public bool CanEquip() => true;
    public IDurabilityState NextOnUse() => new WornState();
    public IDurabilityState Repair() => this;
    public double EffectivenessMultiplier => 1.0;
}

public sealed class WornState : IDurabilityState
{
    public string Name => "worn";
    public bool CanEquip() => true;
    public IDurabilityState NextOnUse() => new BrokenState();
    public IDurabilityState Repair() => new PristineState();
    public double EffectivenessMultiplier => 0.85;
}

public sealed class BrokenState : IDurabilityState
{
    public string Name => "broken";
    public bool CanEquip() => false;
    public IDurabilityState NextOnUse() => this;
    public IDurabilityState Repair() => new WornState();
    public double EffectivenessMultiplier => 0.0;
}
