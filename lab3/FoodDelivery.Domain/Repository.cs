
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FoodDelivery.Domain;

/// <summary>Repository: абстракция хранилища заказов.</summary>
public abstract class OrderRepository
{
    public abstract void Save(Order order);
    public abstract Order? Get(string orderId);
    public abstract IReadOnlyList<Order> ListByStatus(string status);
}

public sealed class InMemoryOrderRepository : OrderRepository
{
    private readonly ConcurrentDictionary<string, Order> _store = new();

    public override void Save(Order order) => _store[order.Id] = order;
    public override Order? Get(string orderId) => _store.TryGetValue(orderId, out var o) ? o : null;
    public override IReadOnlyList<Order> ListByStatus(string status)
        => _store.Values.Where(o => o.State == status).ToList();
}
