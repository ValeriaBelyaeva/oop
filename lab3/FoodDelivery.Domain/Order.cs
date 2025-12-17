
using System;
using System.Collections.Generic;

namespace FoodDelivery.Domain;

public record OrderItem(MenuItem Item, int Quantity);

/// управляем подпищиками состояниями и ценой
public sealed class Order
{
    private readonly List<IOrderObserver> _observers = new();
    private OrderState _state;
    private readonly PricingRule _pricingPipeline;

    public string Id { get; }
    public List<OrderItem> Items { get; }
    public string? PromoCode { get; }
    public Dictionary<string, object> Meta { get; }
    public DateTime CreatedAt { get; }
    public List<string> History { get; } = new();

    public IDeliveryFeeStrategy DeliveryStrategy { get; }
    public ITaxStrategy TaxStrategy { get; }
    public double BaseDistanceKm { get; }

    public Order(string id, IEnumerable<OrderItem> items, IDeliveryFeeStrategy deliveryStrategy, ITaxStrategy taxStrategy, double baseDistanceKm, string? promoCode = null, Dictionary<string, object>? meta = null)
    {
        Id = id;
        Items = new(items);
        PromoCode = promoCode;
        Meta = meta ?? new();
        CreatedAt = DateTime.UtcNow;
        _state = new Created();
        AppendHistory(_state.Name);
        _pricingPipeline = Pricing.BuildDefaultPipeline();
        DeliveryStrategy = deliveryStrategy;
        TaxStrategy = taxStrategy;
        BaseDistanceKm = baseDistanceKm;
    }

    public string State => _state.Name;

    internal void AppendHistory(string stateName) => History.Add(stateName);
    internal void SetState(OrderState newState) => _state = newState;

    // ===== Observer =====
    public void AddObserver(IOrderObserver obs) => _observers.Add(obs);
    public void RemoveObserver(IOrderObserver obs) => _observers.Remove(obs);
    private void Notify(string evt)
    {
        foreach (var o in _observers.ToArray()) o.Update(this, evt);
    }

    // ===== State transitions =====
    public void ToPreparing()
    {
        var prev = State;
        _state.Transition(this, typeof(Preparing));
        Notify($"status:{prev}->{State}");
    }

    public void ToDelivering()
    {
        var prev = State;
        _state.Transition(this, typeof(Delivering));
        Notify($"status:{prev}->{State}");
    }

    public void ToCompleted()
    {
        var prev = State;
        _state.Transition(this, typeof(Completed));
        Notify($"status:{prev}->{State}");
    }

    public void Cancel()
    {
        var prev = State;
        _state.Transition(this, typeof(Cancelled));
        Notify($"status:{prev}->{State}");
    }

    // ===== Pricing =====
    public PriceBreakdown CalculateTotal()
    {
        var ctx = new PricingContext(this, DeliveryStrategy, TaxStrategy, BaseDistanceKm);
        _pricingPipeline.Handle(ctx);
        return ctx.Breakdown;
    }
}
