
using System;
using Xunit;
using FoodDelivery.Domain;

namespace FoodDelivery.Tests;

public class StateTests
{
    [Fact]
    public void HappyPath_Transitions_Succeed()
    {
        var pizza = new MenuItem("p", "Pizza", 400.00m);
        var order = new OrderBuilder("o2").AddItem(pizza, 1).Build();
        Assert.Equal("CREATED", order.State);
        order.ToPreparing();
        Assert.Equal("PREPARING", order.State);
        order.ToDelivering();
        Assert.Equal("DELIVERING", order.State);
        order.ToCompleted();
        Assert.Equal("COMPLETED", order.State);
        Assert.Equal(new[] {"CREATED", "PREPARING", "DELIVERING", "COMPLETED"}, order.History);
    }

    [Fact]
    public void InvalidTransition_Throws()
    {
        var burger = new MenuItem("b", "Burger", 300.00m);
        var order = new OrderBuilder("o3").AddItem(burger, 1).Build();
        Assert.Throws<InvalidTransition>(() => order.ToDelivering()); // from CREATED to DELIVERING is forbidden
        order.Cancel();
        Assert.Equal("CANCELLED", order.State);
        Assert.Throws<InvalidTransition>(() => order.ToPreparing()); // terminal
    }
}
