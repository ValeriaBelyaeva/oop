using System;
using System.Collections.Concurrent;
using RpgInventory.Models;

namespace RpgInventory.Repositories;

public sealed class InMemoryItemRepository : IItemRepository
{
    private readonly ConcurrentDictionary<Guid, Item> _items = new();

    public double TotalWeight => _items.Values.Sum(i => i.Weight);

    public void Add(Item item) => _items[item.Id] = item;

    public Item Remove(Guid id)
    {
        if (_items.TryRemove(id, out var removed))
            return removed;
        throw new KeyNotFoundException($"Item {id} not found");
    }

    public bool TryGet(Guid id, out Item? item) => _items.TryGetValue(id, out item);

    public IEnumerable<Item> All() => _items.Values.ToArray();
}
