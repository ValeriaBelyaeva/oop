using System;
using RpgInventory.Models;

namespace RpgInventory.Repositories;

public interface IItemRepository
{
    void Add(Item item);
    Item Remove(Guid id);
    bool TryGet(Guid id, out Item? item);
    IEnumerable<Item> All();
    double TotalWeight { get; }
}
