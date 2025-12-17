
using System.Collections.Generic;
using System.Linq;

namespace FoodDelivery.Domain;

public record MenuItem(string Id, string Name, decimal Price);

public class Menu
{
    private readonly Dictionary<string, MenuItem> _items = new();

    public Menu(IEnumerable<MenuItem>? items = null)
    {
        if (items != null)
        {
            foreach (var i in items) _items[i.Id] = i;
        }
    }

    public void AddItem(MenuItem item) => _items[item.Id] = item;
    public MenuItem Get(string id) => _items[id];
    public IReadOnlyCollection<MenuItem> All() => _items.Values.ToList().AsReadOnly();
}
