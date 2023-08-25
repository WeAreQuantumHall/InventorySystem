using System;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;

namespace InventorySystem.Inventories
{
    public class Inventory : InventoryBase
    {
        public override int Count => Items.Count;

        public override bool IsAtCapacity => Capacity > 0 && Count == Capacity;

        public Inventory(string name, int capacity = 0) : base(name, capacity)
            => Items.EnsureCapacity(capacity);
        
        public override IInventoryActionResult TryAddItem(IItem item)
        {
            var(action, returnedItem) = item.Stackable
                ? TryAddStackableItem(item)
                : TryAddItemToDictionary(item);
            
            return new InventoryActionResult(action, returnedItem);
        }

        public override IInventoryActionResult TryGetItem(Guid id) 
        {
            var (action, returnedItem) = TryGetItemFromDictionary(id);
            return new InventoryActionResult(action, returnedItem);
        }

        public override IInventoryActionResult TryGetItemsByCategory(ItemCategory category)
        {
            var (action, result) = TryGetItemsFromDictionaryByCategory(category);
            return new InventoryActionResult(action, result);
        }

        public override IInventoryActionResult TryGetAllItems()
        {
            var (action, result) = TryGetAllItemsFromDictionary();
            return new InventoryActionResult(action, result);
        } 
        
        public override IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
        {
            var (action, result) = TrySplitItemInDictionary(id, splitAmount);
            return new InventoryActionResult(action, result);
        }

        public override IInventoryActionResult TryRemoveItem(Guid id)
        {
            var (action, item) = TryRemoveItemFromDictionary(Items, id);
            return new InventoryActionResult(action, item); 
        }
        
        public override IInventoryActionResult TryGetItemsByTag(string tag)
        {
            var (action, item) = TryGetItemsFromDictionaryByTag(Items, tag);
            return new InventoryActionResult(action, item); 
        }
    }
}