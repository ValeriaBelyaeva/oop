
using System;
using System.Collections.Generic;

namespace FoodDelivery.Domain;

public sealed class OrderBuilder
{
    private readonly string _orderId;
    private readonly List<OrderItem> _items = new();
    private string? _promoCode;
    private readonly Dictionary<string, object> _meta = new();
    private double _baseDistanceKm = 2.0;
    private string _speed = "standard";
    private bool _contactless = false;
    private bool _green = false;
    private string _region = "RU";

    public OrderBuilder(string orderId) { _orderId = orderId; }

    public OrderBuilder AddItem(MenuItem item, int qty) { _items.Add(new OrderItem(item, qty)); return this; }
    public OrderBuilder SetDistance(double km) { _baseDistanceKm = km; return this; }
    public OrderBuilder UsePromo(string code) { _promoCode = code; return this; }
    public OrderBuilder WithSpeed(string speed) { if (speed != "standard" && speed != "express") throw new ArgumentException("speed must be 'standard' or 'express'"); _speed = speed; return this; }
    public OrderBuilder ContactlessDelivery(bool value = true) { _contactless = value; return this; }
    public OrderBuilder GreenDelivery(bool value = true) { _green = value; return this; }
    public OrderBuilder SetRegion(string region) { _region = region; return this; }
    public OrderBuilder SetMeta(string key, object value) { _meta[key] = value; return this; }

    public Order Build()
    {
        var delivery = DeliveryStrategyFactory.Create(_speed, _contactless, _green);
        var tax = TaxStrategyFactory.Create(_region);
        var p = new OrderParams
        {
            Id = _orderId,
            Items = _items,
            BaseDistanceKm = _baseDistanceKm,
            PromoCode = _promoCode,
            Meta = _meta
        };
        return OrderFactory.Create(p, delivery, tax);
    }
}
