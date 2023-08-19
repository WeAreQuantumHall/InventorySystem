using InventorySystem.Abstractions;

namespace InventorySystem.Factories
{
    public static class ItemFactory
    {
        public static IItem CreateItem(string name, string identifier)
            => new Item
            {
                Name = name,
                Identifier = identifier,
                Stackable = false,
                CurrentAmount = 0,
                MaxAmount = 0
            };

        public static IItem CreateStackableItem(string name, string identifier, bool stackable, int currentAmount,
            int maxAmount)
            
            => new Item
            {
                Name = name,
                Identifier = identifier,
                Stackable = stackable,
                CurrentAmount = currentAmount,
                MaxAmount = maxAmount
            };

    }
}