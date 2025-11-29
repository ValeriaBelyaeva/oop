
using Xunit;
using FoodDelivery.Domain;
using System.Linq;

namespace FoodDelivery.Tests;

public class ObserverTests
{
    [Fact]
    public void Observer_Receives_StatusUpdates()
    {
        var item = new MenuItem("p", "Pizza", 400.00m);
        var order = new OrderBuilder("o4").AddItem(item, 1).Build();
        var obs = new MemoryObserver();
        order.AddObserver(obs);
        order.ToPreparing();
        order.ToDelivering();
        var events = obs.Events.Select(e => e.Event).ToList();
        Assert.Contains("status:CREATED->PREPARING", events[0]);
        Assert.Contains("status:PREPARING->DELIVERING", events[1]);
    }
}
