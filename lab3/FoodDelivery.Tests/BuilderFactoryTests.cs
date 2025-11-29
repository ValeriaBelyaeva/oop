
using Xunit;
using FoodDelivery.Domain;

namespace FoodDelivery.Tests;

public class BuilderFactoryTests
{
    [Fact]
    public void DeliveryStrategyFactory_Composes_Decorators()
    {
        var strat = DeliveryStrategyFactory.Create(speed: "express", contactless: true, green: true);
        Assert.IsType<GreenDeliveryDecorator>(strat);
        var inner1 = (strat as GreenDeliveryDecorator)!.InnerStrategy;
        Assert.IsType<ContactlessDeliveryDecorator>(inner1);
        var inner2 = (inner1 as ContactlessDeliveryDecorator)!.InnerStrategy;
        Assert.IsType<ExpressDeliveryDecorator>(inner2);
        var inner3 = (inner2 as ExpressDeliveryDecorator)!.InnerStrategy;
        Assert.IsType<BaseDeliveryFee>(inner3);
    }

    [Fact]
    public void TaxStrategyFactory_Works()
    {
        Assert.IsType<FlatRateTax>(TaxStrategyFactory.Create("RU"));
        Assert.IsType<NoTax>(TaxStrategyFactory.Create("DE")); // для неизвестного региона по умолчанию NoTax
    }

    [Fact]
    public void Builder_Constructs_Order()
    {
        var item = new MenuItem("t", "Tea", 120.00m);
        var order = new OrderBuilder("b1").AddItem(item, 2).WithSpeed("standard").SetRegion("US").Build();
        Assert.Equal("b1", order.Id);
        Assert.Single(order.Items);
        Assert.Equal(2, order.Items[0].Quantity);
    }
}
