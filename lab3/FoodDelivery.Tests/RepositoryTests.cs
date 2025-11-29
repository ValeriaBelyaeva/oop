
using Xunit;
using FoodDelivery.Domain;
using System.Linq;

namespace FoodDelivery.Tests;

public class RepositoryTests
{
    [Fact]
    public void Save_Get_ListByStatus_Works()
    {
        var repo = new InMemoryOrderRepository();
        var item = new MenuItem("s", "Soup", 250.00m);
        var order1 = new OrderBuilder("r1").AddItem(item, 2).Build();
        var order2 = new OrderBuilder("r2").AddItem(item, 1).Build();
        repo.Save(order1);
        repo.Save(order2);
        Assert.NotNull(repo.Get("r1"));
        order1.ToPreparing();
        repo.Save(order1);
        var listed = repo.ListByStatus("PREPARING").Select(o => o.Id).ToList();
        Assert.Equal(new[] { "r1" }, listed);
    }
}
