using RpgInventory.Models;

namespace RpgInventory.Factories;

public class MedievalItemFactory : IItemFactory
    {
        public Weapon CreateWeapon(Rarity rarity = Rarity.Common)
        {
            var baseDamage = rarity switch
            {
                Rarity.Common => 12,
                Rarity.Rare => 18,
                Rarity.Epic => 28,
                Rarity.Legendary => 40,
                _ => 12
            };
            return new Weapon { Name = "Iron Sword", Weight = 4.5, Rarity = rarity, BaseDamage = baseDamage, Slot = Slot.MainHand };
        }

        public Armor CreateArmor(Rarity rarity = Rarity.Common)
        {
            var baseDefense = rarity switch
            {
                Rarity.Common => 8,
                Rarity.Rare => 14,
                Rarity.Epic => 22,
                Rarity.Legendary => 32,
                _ => 8
            };
            return new Armor { Name = "Chainmail", Weight = 10.0, Rarity = rarity, BaseDefense = baseDefense, Slot = Slot.Chest };
        }

        public Potion CreatePotion(Rarity rarity = Rarity.Common)
        {
            var potency = rarity switch
            {
                Rarity.Common => 25,
                Rarity.Rare => 40,
                Rarity.Epic => 60,
                Rarity.Legendary => 100,
                _ => 25
            };
            return new Potion { Name = "Healing Potion", Weight = 0.3, Rarity = rarity, Effect = "heal", Potency = potency, Quantity = 1 };
        }

        public QuestItem CreateQuestItem()
        {
            return new QuestItem { Name = "Royal Seal", Weight = 0.1, Rarity = Rarity.Rare, Description = "Authority of the crown." };
    }
}
