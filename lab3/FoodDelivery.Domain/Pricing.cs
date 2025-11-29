
using System;
using System.Linq;

namespace FoodDelivery.Domain;

/// <summary>DTO с детализацией цены.</summary>
public sealed class PriceBreakdown
{
    public decimal Subtotal { get; set; } = 0.00m;
    public decimal Discounts { get; set; } = 0.00m;
    public decimal ServiceFee { get; set; } = 0.00m;
    public decimal DeliveryFee { get; set; } = 0.00m;
    public decimal Tax { get; set; } = 0.00m;
    public decimal Total { get; set; } = 0.00m;
}

public sealed class PricingContext
{
    public Order Order { get; }
    public IDeliveryFeeStrategy DeliveryStrategy { get; }
    public ITaxStrategy TaxStrategy { get; }
    public double BaseDistanceKm { get; }
    public PriceBreakdown Breakdown { get; } = new();

    public PricingContext(Order order, IDeliveryFeeStrategy deliveryStrategy, ITaxStrategy taxStrategy, double baseDistanceKm)
    {
        Order = order;
        DeliveryStrategy = deliveryStrategy;
        TaxStrategy = taxStrategy;
        BaseDistanceKm = baseDistanceKm;
    }
}

/// <summary>Chain of Responsibility: правила ценообразования.</summary>
public abstract class PricingRule
{
    public PricingRule? Next { get; private set; }

    public PricingRule SetNext(PricingRule next)
    {
        Next = next;
        return next;
    }

    public void Handle(PricingContext ctx)
    {
        Apply(ctx);
        Next?.Handle(ctx);
    }

    protected abstract void Apply(PricingContext ctx);
}

public sealed class SubtotalRule : PricingRule
{
    protected override void Apply(PricingContext ctx)
    {
        ctx.Breakdown.Subtotal = ctx.Order.Items.Aggregate(0.00m, (acc, i) => acc + i.Item.Price * i.Quantity);
    }
}

public sealed class PromoPercentRule : PricingRule
{
    private readonly decimal _percent;
    private readonly decimal _minSubtotal;
    public PromoPercentRule(decimal percent, decimal minSubtotal = 0.00m)
    {
        _percent = percent; _minSubtotal = minSubtotal;
    }

    protected override void Apply(PricingContext ctx)
    {
        if (ctx.Breakdown.Subtotal >= _minSubtotal && !string.IsNullOrWhiteSpace(ctx.Order.PromoCode))
        {
            var discount = MoneyUtils.Round(ctx.Breakdown.Subtotal * _percent);
            ctx.Breakdown.Discounts += discount;
        }
    }
}

public sealed class BulkDiscountRule : PricingRule
{
    private readonly int _thresholdQty;
    private readonly decimal _amount;
    public BulkDiscountRule(int thresholdQty, decimal amount)
    {
        _thresholdQty = thresholdQty; _amount = amount;
    }

    protected override void Apply(PricingContext ctx)
    {
        var totalQty = ctx.Order.Items.Sum(i => i.Quantity);
        if (totalQty >= _thresholdQty) ctx.Breakdown.Discounts += _amount;
    }
}

public sealed class ServiceFeeRule : PricingRule
{
    private readonly decimal _percent;
    private readonly decimal? _cap;
    public ServiceFeeRule(decimal percent, decimal? cap = null)
    {
        _percent = percent; _cap = cap;
    }

    protected override void Apply(PricingContext ctx)
    {
        var fee = MoneyUtils.Round(ctx.Breakdown.Subtotal * _percent);
        if (_cap.HasValue) fee = Math.Min(fee, _cap.Value);
        ctx.Breakdown.ServiceFee += fee;
    }
}

public sealed class DeliveryFeeRule : PricingRule
{
    protected override void Apply(PricingContext ctx)
    {
        ctx.Breakdown.DeliveryFee = ctx.DeliveryStrategy.ComputeFee(ctx.BaseDistanceKm);
    }
}

public sealed class TaxRule : PricingRule
{
    protected override void Apply(PricingContext ctx)
    {
        var taxable = Math.Max(0.00m, ctx.Breakdown.Subtotal - ctx.Breakdown.Discounts);
        ctx.Breakdown.Tax = ctx.TaxStrategy.ComputeTax(taxable);
    }
}

public sealed class TotalRule : PricingRule
{
    protected override void Apply(PricingContext ctx)
    {
        var b = ctx.Breakdown;
        b.Total = MoneyUtils.Round(b.Subtotal - b.Discounts + b.ServiceFee + b.DeliveryFee + b.Tax);
    }
}

public static class Pricing
{
    public static PricingRule BuildDefaultPipeline()
    {
        var chain = new SubtotalRule();
        chain.SetNext(new PromoPercentRule(percent: 0.10m, minSubtotal: 1000.00m))
             .SetNext(new BulkDiscountRule(thresholdQty: 5, amount: 100.00m))
             .SetNext(new ServiceFeeRule(percent: 0.05m, cap: 150.00m))
             .SetNext(new DeliveryFeeRule())
             .SetNext(new TaxRule())
             .SetNext(new TotalRule());
        return chain;
    }
}
