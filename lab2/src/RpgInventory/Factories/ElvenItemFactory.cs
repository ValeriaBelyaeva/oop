using RpgInventory.Models;

namespace RpgInventory.Factories;

public class ElvenItemFactory : IItemFactory
    {
        public Weapon CreateWeapon(Rarity rarity = Rarity.Common)
        {
            var baseDamage = rarity switch
            {
                Rarity.Common => 10,
                Rarity.Rare => 16,
                Rarity.Epic => 24,
                Rarity.Legendary => 36,
                _ => 10
            };
            return new Weapon { Name = "Elven Blade", Weight = 3.2, Rarity = rarity, BaseDamage = baseDamage, Slot = Slot.MainHand };
        }

        public Armor CreateArmor(Rarity rarity = Rarity.Common)
        {
            var baseDefense = rarity switch
            {
                Rarity.Common => 6,
                Rarity.Rare => 12,
                Rarity.Epic => 20,
                Rarity.Legendary => 30,
                _ => 6
            };
            return new Armor { Name = "Leafweave", Weight = 6.5, Rarity = rarity, BaseDefense = baseDefense, Slot = Slot.Chest };
        }

        public Potion CreatePotion(Rarity rarity = Rarity.Common)
        {
            var potency = rarity switch
            {
                Rarity.Common => 20,
                Rarity.Rare => 35,
                Rarity.Epic => 55,
                Rarity.Legendary => 90,
                _ => 20
            };
            return new Potion { Name = "Elixir of Grace", Weight = 0.2, Rarity = rarity, Effect = "haste", Potency = potency, Quantity = 1 };
        }

        public QuestItem CreateQuestItem()
        {
            return new QuestItem { Name = "Moon Sigil", Weight = 0.05, Rarity = Rarity.Epic, Description = "Mark of the Silver Court." };
    }
}
