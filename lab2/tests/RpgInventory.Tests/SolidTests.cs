using System;
using RpgInventory;
using RpgInventory.Models;
using RpgInventory.Builders;
using RpgInventory.Services;
using RpgInventory.Strategies;
using RpgInventory.Repositories;
using Xunit;

namespace RpgInventory.Tests;

public class SolidTests
{
    private sealed class DummyWeaponBoost : IUpgradeStrategy
    {
        public bool AppliesTo(Item item) => item is Weapon;
        public void Apply(Item item)
        {
            if (item is Weapon w) w.BaseDamage += 3;
        }
    }

    [Fact]
    public void SRP_InventoryManagesItems()
    {
        var inv = new Inventory(new InMemoryItemRepository());
        var w = new Weapon("SRP Sword", 5, Rarity.Common, 10);
        inv.Add(w);
        Assert.Equal("SRP Sword", inv.Get(w.Id).Name);
    }

    [Fact]
    public void OCP_AddNewStrategyWithoutChanges()
    {
        var w = new Weapon("OCP Sword", 5, Rarity.Common, 10);
        var service = new UpgradeService(new[] { new DummyWeaponBoost() });
        service.Upgrade(w);
        Assert.Equal(13, w.BaseDamage);
    }

    [Fact]
    public void LSP_InventoryAcceptsBaseItems()
    {
        var inv = new Inventory(new InMemoryItemRepository());
        Item[] items = new Item[]
        {
            new Weapon("LSP Sword", 5, Rarity.Rare, 12),
            new Armor("LSP Armor", 8, Rarity.Common, 5),
            new Potion("LSP Potion", 0.1, Rarity.Rare, "heal", 30, 1),
        };
        foreach (var it in items) inv.Add(it);
        Assert.Equal(3, inv.Items().Count);
    }

    [Fact]
    public void ISP_SegregatedInterfaces()
    {
        var w = new Weapon("ISP Sword", 5, Rarity.Common, 10);
        var p = new Potion("ISP Potion", 0.1, Rarity.Common, "heal", 10, 1);
        Assert.True(w is IEquippable);
        Assert.True(p is IConsumable);
    }

    [Fact]
    public void DIP_UpgradeServiceDependsOnAbstraction()
    {
        var w = new Weapon("DIP Sword", 5, Rarity.Common, 10);
        var service = new UpgradeService(new[] { new DummyWeaponBoost() });
        service.Upgrade(w);
        Assert.Equal(13, w.BaseDamage);
    }
}
