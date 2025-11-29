using RpgInventory;
using RpgInventory.Models;
using RpgInventory.Builders;
using RpgInventory.Factories;
using RpgInventory.Services;
using RpgInventory.Strategies;
using Xunit;

namespace RpgInventory.Tests;

public class PatternsTests
{
    [Fact]
    public void Builder_Works()
    {
        var sword = new WeaponBuilder()
            .WithName("Claymore")
            .WithWeight(6.5)
            .WithRarity(Rarity.Rare)
            .WithDamage(22)
            .Build();

        Assert.Equal("Claymore", sword.Name);
        Assert.Equal(22, sword.BaseDamage);
        Assert.Equal(Rarity.Rare, sword.Rarity);
    }

    [Fact]
    public void AbstractFactory_Works()
    {
        var m = new MedievalItemFactory();
        var e = new ElvenItemFactory();
        Assert.Equal("Iron Sword", m.CreateWeapon(Rarity.Common).Name);
        Assert.Equal("Elven Blade", e.CreateWeapon(Rarity.Rare).Name);
        Assert.Equal("heal", m.CreatePotion(Rarity.Epic).Effect);
        Assert.Equal("haste", e.CreatePotion(Rarity.Epic).Effect);
    }

    [Fact]
    public void Strategy_Upgrades_Work()
    {
        var sword = new WeaponBuilder().WithDamage(20).Build();
        var armor = new ArmorBuilder().WithDefense(10).Build();
        var service = new UpgradeService(new IUpgradeStrategy[]
        {
            new SharpnessUpgradeStrategy(),
            new RuneEtchStrategy(),
            new ReinforcePlatesStrategy()
        });

        Assert.True(service.Upgrade(sword));
        Assert.True(service.Upgrade(armor));
        Assert.True(sword.BaseDamage >= 20);
        Assert.True(armor.BaseDefense >= 10);
    }

    [Fact]
    public void State_Transitions_Work()
    {
        var sword = new WeaponBuilder().WithDamage(10).Build();
        Assert.IsType<PristineState>(sword.Durability);
        sword.ApplyWear();
        Assert.IsType<WornState>(sword.Durability);
    }

    [Fact]
    public void Combine_Potions_Works()
    {
        var pa = new Potion("Healing Potion", 0.3, Rarity.Rare, "heal", 40, 1);
        var pb = new Potion("Healing Potion", 0.3, Rarity.Rare, "heal", 40, 1);
        var combined = new CombineService(new PotencyCombineStrategy()).CombinePotions(pa, pb);
        Assert.Equal(80, combined.Potency);
        Assert.Equal(1, combined.Quantity);
    }
}
