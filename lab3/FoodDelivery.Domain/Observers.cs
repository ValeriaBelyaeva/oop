
using System.Collections.Generic;

namespace FoodDelivery.Domain;

/// подписка на события заказа
public interface IOrderObserver
{
    void Update(Order order, string @event);
}

public sealed class MemoryObserver : IOrderObserver
{
    public List<(string OrderId, string Event)> Events { get; } = new();

    public void Update(Order order, string @event)
    {
        Events.Add((order.Id, @event));
    }
}
