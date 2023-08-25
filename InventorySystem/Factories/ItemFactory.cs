using System.Collections.Generic;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Items;
using InventorySystem.Tags;

namespace InventorySystem.Factories
{
    /// <summary>
    /// Provides methods for creating inventory items.
    /// </summary>
    public static class ItemFactory
    {
        /// <summary>
        /// Creates a new non-stackable item with the specified identifier and name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="tags"></param>
        /// <returns>A new instance of an inventory item.</returns>
        public static IItem CreateItem(string name, IEnumerable<ITag> tags)
            => new Item(name, new TagList(tags))
            {
                Stackable = false,
                Stack = 0,
                MaxStack = 0
            };

        /// <summary>
        /// Creates a new stackable item with the specified properties.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="stackable">Whether the item is stackable.</param>
        /// <param name="currentAmount">The current amount in the stack.</param>
        /// <param name="maxAmount">The maximum amount the stack can hold.</param>
        /// <param name="tags"></param>
        /// <returns>A new instance of a stackable item.</returns>
        public static IItem CreateStackableItem(
            string name,
            bool stackable,
            int currentAmount,
            int maxAmount,
            IEnumerable<ITag> tags)
            => new Item(name, new TagList(tags))
            {
                Stackable = stackable,
                Stack = currentAmount,
                MaxStack = maxAmount
            };
    }
}