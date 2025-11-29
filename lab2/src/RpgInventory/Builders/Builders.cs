using RpgInventory;
using RpgInventory.Models;

namespace RpgInventory.Builders;

public sealed class WeaponBuilder
{
    private string _name = "Unnamed Weapon";
    private double _weight = 0.0;
    private Rarity _rarity = Rarity.Common;
    private int _baseDamage = 0;
    private Slot _slot = Slot.MainHand;

    public WeaponBuilder WithName(string name) { _name = name; return this; }
    public WeaponBuilder WithWeight(double weight) { _weight = weight; return this; }
    public WeaponBuilder WithRarity(Rarity rarity) { _rarity = rarity; return this; }
    public WeaponBuilder WithDamage(int dmg) { _baseDamage = dmg; return this; }
    public WeaponBuilder WithSlot(Slot slot) { _slot = slot; return this; }

    public Weapon Build() => new Weapon { Name = _name, Weight = _weight, Rarity = _rarity, BaseDamage = _baseDamage, Slot = _slot };
}

public sealed class ArmorBuilder
{
    private string _name = "Unnamed Armor";
    private double _weight = 0.0;
    private Rarity _rarity = Rarity.Common;
    private int _baseDefense = 0;
    private Slot _slot = Slot.Chest;

    public ArmorBuilder WithName(string name) { _name = name; return this; }
    public ArmorBuilder WithWeight(double weight) { _weight = weight; return this; }
    public ArmorBuilder WithRarity(Rarity rarity) { _rarity = rarity; return this; }
    public ArmorBuilder WithDefense(int value) { _baseDefense = value; return this; }
    public ArmorBuilder WithSlot(Slot slot) { _slot = slot; return this; }

    public Armor Build() => new Armor { Name = _name, Weight = _weight, Rarity = _rarity, BaseDefense = _baseDefense, Slot = _slot };
}

public sealed class PotionBuilder
{
    private string _name = "Potion";
    private double _weight = 0.1;
    private Rarity _rarity = Rarity.Common;
    private string _effect = "heal";
    private int _potency = 25;
    private int _quantity = 1;

    public PotionBuilder WithName(string name) { _name = name; return this; }
    public PotionBuilder WithWeight(double weight) { _weight = weight; return this; }
    public PotionBuilder WithRarity(Rarity rarity) { _rarity = rarity; return this; }
    public PotionBuilder WithEffect(string effect) { _effect = effect; return this; }
    public PotionBuilder WithPotency(int potency) { _potency = potency; return this; }
    public PotionBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

    public Potion Build() => new Potion { Name = _name, Weight = _weight, Rarity = _rarity, Effect = _effect, Potency = _potency, Quantity = _quantity };
}

public sealed class QuestItemBuilder
{
    private string _name = "Quest Item";
    private double _weight = 0.0;
    private Rarity _rarity = Rarity.Common;
    private string _description = "";

    public QuestItemBuilder WithName(string name) { _name = name; return this; }
    public QuestItemBuilder WithWeight(double weight) { _weight = weight; return this; }
    public QuestItemBuilder WithRarity(Rarity rarity) { _rarity = rarity; return this; }
    public QuestItemBuilder WithDescription(string description) { _description = description; return this; }

    public QuestItem Build() => new QuestItem { Name = _name, Weight = _weight, Rarity = _rarity, Description = _description };
}
