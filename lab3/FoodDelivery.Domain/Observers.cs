
using System.Collections.Generic;

namespace FoodDelivery.Domain;

/// <summary>Паттерн Observer: подписка на события заказа.</summary>
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
