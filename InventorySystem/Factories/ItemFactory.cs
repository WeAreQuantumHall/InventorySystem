using System.Collections.Generic;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Items;
using InventorySystem.Tags;

namespace InventorySystem.Factories
{
    public static class ItemFactory
    {
        public static IItem CreateItem(string name, IEnumerable<ITag> tags, IItemStack? itemStack = null)
            => new Item(name, new TagList(tags))
            {
                ItemStack = itemStack == null 
                    ? null 
                    : new ItemStack(itemStack.Current, itemStack.Max)
            };
    }
}