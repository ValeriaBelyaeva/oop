
using System;
using Xunit;
using FoodDelivery.Domain;

namespace FoodDelivery.Tests;

public class PricingTests
{
    [Fact]
    public void Pricing_WithPromoBulkAndFees_Works()
    {
        var pizza = new MenuItem("p1", "Pizza", 500.00m);
        var soda = new MenuItem("s1", "Soda", 100.00m);

        var order =
            new OrderBuilder("o1")
                .AddItem(pizza, 2)     // 1000
                .AddItem(soda, 3)      // +300 -> 1300
                .WithSpeed("express")  // express
                .ContactlessDelivery(true) // +20
                .GreenDelivery(true)   // -10% delivery
                .SetDistance(5.0)      // base delivery: 150 + 20*(5-2)=210
                .UsePromo("PROMO10")   // 10% promo
                .SetRegion("RU")       // 20% VAT
                .Build();

        var br = order.CalculateTotal();

        Assert.Equal(1300.00m, br.Subtotal);
        Assert.Equal(230.00m, br.Discounts);     // 130 + 100
        Assert.Equal(65.00m, br.ServiceFee);     // 5% of 1300
        Assert.Equal(387.00m, br.DeliveryFee);   // (210 + 200 + 20) * 0.9
        Assert.Equal(214.00m, br.Tax);           // 20% of (1300-230)
        Assert.Equal(1736.00m, br.Total);        // 1300 - 230 + 65 + 387 + 214
    }
}
