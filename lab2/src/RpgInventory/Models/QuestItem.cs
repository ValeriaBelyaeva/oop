using System.Collections.Generic;

namespace RpgInventory.Models;

public sealed class QuestItem : Item
{
    public string Description { get; set; } = string.Empty;

    public QuestItem() {}
    
    public QuestItem(string name, double weight, Rarity rarity, string description)
    {
        Name = name;
        Weight = weight;
        Rarity = rarity;
        Description = description;
    }

    public override Dictionary<string, object?> Info() => new()
    {
        ["type"] = "quest_item",
        ["id"] = Id,
        ["name"] = Name,
        ["weight"] = Weight,
        ["rarity"] = Rarity.ToString(),
        ["description"] = Description
    };
}