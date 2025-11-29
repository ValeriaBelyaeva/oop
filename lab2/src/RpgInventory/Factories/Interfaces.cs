using RpgInventory.Models;

namespace RpgInventory.Factories;

public interface IItemFactory
{
    Weapon CreateWeapon(Rarity rarity = Rarity.Common);
    Armor CreateArmor(Rarity rarity = Rarity.Common);
    Potion CreatePotion(Rarity rarity = Rarity.Common);
    QuestItem CreateQuestItem();
}

