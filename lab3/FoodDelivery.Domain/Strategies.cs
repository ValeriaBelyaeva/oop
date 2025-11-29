
using System;

namespace FoodDelivery.Domain;

/// <summary>Утилиты округления денежных значений.</summary>
public static class MoneyUtils
{
    public static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

/// <summary>Strategy: налоговые стратегии.</summary>
public interface ITaxStrategy
{
    decimal ComputeTax(decimal taxableAmount);
}

public sealed class NoTax : ITaxStrategy
{
    public decimal ComputeTax(decimal taxableAmount) => 0.00m;
}

public sealed class FlatRateTax : ITaxStrategy
{
    public decimal Rate { get; }
    public FlatRateTax(decimal rate) { Rate = rate; }
    public decimal ComputeTax(decimal taxableAmount) => MoneyUtils.Round(taxableAmount * Rate);
}

/// <summary>Strategy: стоимость доставки.</summary>
public interface IDeliveryFeeStrategy
{
    decimal ComputeFee(double baseDistanceKm);
}

public sealed class BaseDeliveryFee : IDeliveryFeeStrategy
{
    /// <summary>База: 150 ₽ + 20 ₽/км после первых 2 км.</summary>
    public decimal ComputeFee(double baseDistanceKm)
    {
        var extraKm = Math.Max(0.0, baseDistanceKm - 2.0);
        var fee = 150.00m + 20.00m * (decimal)extraKm;
        return MoneyUtils.Round(fee);
    }
}

/// <summary>Decorator: обёртка для опций доставки.</summary>
public abstract class DeliveryFeeDecorator : IDeliveryFeeStrategy
{
    protected readonly IDeliveryFeeStrategy Inner;
    protected DeliveryFeeDecorator(IDeliveryFeeStrategy inner) => Inner = inner;

    // Делаем InnerStrategy доступным для тестов/диагностики
    public IDeliveryFeeStrategy InnerStrategy => Inner;

    public abstract decimal ComputeFee(double baseDistanceKm);
}

public sealed class ExpressDeliveryDecorator : DeliveryFeeDecorator
{
    public ExpressDeliveryDecorator(IDeliveryFeeStrategy inner) : base(inner) { }
    public override decimal ComputeFee(double baseDistanceKm) => Inner.ComputeFee(baseDistanceKm) + 200.00m;
}

public sealed class ContactlessDeliveryDecorator : DeliveryFeeDecorator
{
    public ContactlessDeliveryDecorator(IDeliveryFeeStrategy inner) : base(inner) { }
    public override decimal ComputeFee(double baseDistanceKm) => Inner.ComputeFee(baseDistanceKm) + 20.00m;
}

public sealed class GreenDeliveryDecorator : DeliveryFeeDecorator
{
    public GreenDeliveryDecorator(IDeliveryFeeStrategy inner) : base(inner) { }
    public override decimal ComputeFee(double baseDistanceKm)
    {
        var baseFee = Inner.ComputeFee(baseDistanceKm);
        return MoneyUtils.Round(baseFee * 0.90m);
    }
}
