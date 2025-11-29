using System;
using RpgInventory;
using RpgInventory.Models;
using RpgInventory.Builders;
using RpgInventory.Services;
using RpgInventory.Repositories;
using Xunit;

namespace RpgInventory.Tests;

public class InventoryTests
{
    private Inventory CreateInventory(double limit = 100.0) => new Inventory(new InMemoryItemRepository(), limit);

    [Fact]
    public void AddAndSnapshot()
    {
        var inv = CreateInventory();
        var sword = new Weapon("Sword", 5.0, Rarity.Common, 10);
        inv.Add(sword);
        var snap = inv.Snapshot();
        Assert.Single(snap.Items);
        Assert.Equal(5.0, snap.TotalWeight, 3);
    }

    [Fact]
    public void EquipWeaponAndApplyWear()
    {
        var inv = CreateInventory();
        var sword = new Weapon("Sword", 5.0, Rarity.Common, 10);
        inv.Add(sword);
        inv.Equip(sword.Id);
        Assert.True(sword.Equipped);
        Assert.Equal(sword.Id, inv.Equipped()[Slot.MainHand] is IEquippable eq ? ((Item)eq).Id : Guid.Empty);

        sword.ApplyWear(); // Pristine -> Worn
        Assert.IsType<WornState>(sword.Durability);
        sword.ApplyWear(); // Worn -> Broken
        Assert.IsType<BrokenState>(sword.Durability);

        inv.Unequip(Slot.MainHand);
        Assert.False(sword.Equipped);

        Assert.Throws<EquipException>(() => inv.Equip(sword.Id));
    }

    [Fact]
    public void UsePotion_RemovesWhenEmpty()
    {
        var inv = CreateInventory();
        var p = new Potion("Health", 0.2, Rarity.Common, "heal", 30, quantity: 2);
        inv.Add(p);

        var result = inv.Use(p.Id);
        Assert.Equal("heal", result.Effect);
        Assert.Equal(30, result.Amount);
        Assert.Equal(1, result.Remaining);

        inv.Use(p.Id); // last one -> auto remove

        Assert.Throws<InventoryException>(() => inv.Get(p.Id));
    }

    [Fact]
    public void WeightLimit()
    {
        var inv = CreateInventory(limit: 100);
        var heavy = new Armor("Heavy", 101.0, Rarity.Rare, 20);
        Assert.Throws<InventoryException>(() => inv.Add(heavy));
    }
}
