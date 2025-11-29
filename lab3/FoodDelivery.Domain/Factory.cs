
using System;
using System.Collections.Generic;

namespace FoodDelivery.Domain;

/// <summary>Factory Method: создаёт стратегию стоимости доставки и применяет декораторы.</summary>
public static class DeliveryStrategyFactory
{
    public static IDeliveryFeeStrategy Create(string speed = "standard", bool contactless = false, bool green = false)
    {
        IDeliveryFeeStrategy strat = new BaseDeliveryFee();
        if (speed == "express")
            strat = new ExpressDeliveryDecorator(strat);
        if (contactless)
            strat = new ContactlessDeliveryDecorator(strat);
        if (green)
            strat = new GreenDeliveryDecorator(strat);
        return strat;
    }
}

public static class TaxStrategyFactory
{
    public static ITaxStrategy Create(string region = "RU")
    {
        var r = region.ToUpperInvariant();
        return r switch
        {
            "RU" or "KZ" => new FlatRateTax(0.20m),
            "US" or "CA" => new FlatRateTax(0.08m),
            _ => new NoTax()
        };
    }
}

public sealed class OrderParams
{
    public string Id { get; init; } = "";
    public List<OrderItem> Items { get; init; } = new();
    public double BaseDistanceKm { get; init; }
    public string? PromoCode { get; init; }
    public Dictionary<string, object>? Meta { get; init; }
}

public static class OrderFactory
{
    public static Order Create(OrderParams p, IDeliveryFeeStrategy deliveryStrategy, ITaxStrategy taxStrategy)
        => new(p.Id, p.Items, deliveryStrategy, taxStrategy, p.BaseDistanceKm, p.PromoCode, p.Meta ?? new());
}
