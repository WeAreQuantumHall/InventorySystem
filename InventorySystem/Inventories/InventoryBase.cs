using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.ActionResults;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Inventories
{
    /// <inheritdoc />
    public abstract class InventoryBase : IInventory
    {
        protected InventoryBase(string name, int capacity)
        {
            Name = name;
            Id = Guid.NewGuid(); 
            Capacity = capacity < 1 ? 0 : capacity;
            Items.EnsureCapacity(capacity);
        }
        
        protected Dictionary<Guid, IItem> Items { get; set; } = new Dictionary<Guid, IItem>();
        
        public Guid Id { get; }
        public string Name { get; private set; }
        public int Capacity { get; }
        
        public virtual int Count => Items.Count;
        public virtual void SetName(string name) => Name = name;

        protected virtual (InventoryAction action, IItem? item) TryAddStackableItem(IItem item)
        {
            var hasSpilled = false;
 
            foreach (var similarItem in TryGetSimilarStackableItems(item.Name))
            {
                var remainingStack = similarItem.AddToStack(item.Stack);
                item.AddToStack(remainingStack);

                if (remainingStack == 0)
                {
                    return hasSpilled
                        ? (ItemStackedAndSpilled, similarItem)
                        : (ItemStacked, similarItem);
                }

                hasSpilled = true;
            }

            return TryAddItemToDictionary(item);
        }

        protected virtual (InventoryAction action, IItem? item) TryGetItemFromDictionary(
            Guid key) 
            => Items.TryGetValue(key, out var retrievedItem)
                ? (ItemRetrieved, retrievedItem)
                : (ItemNotFound, null);
        

        protected virtual (InventoryAction action, IEnumerable<IItem> item) TryGetAllItemsFromDictionary()
        {
            var retrievedItems = Items.Values
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }
        protected virtual IEnumerable<IItem> TryGetSimilarStackableItems(
            string name)  
            => Items.Values
                .Where(i => i.Name == name && i.CanBeStackedOn);

        protected virtual (InventoryAction action, IItem item) TryAddItemToDictionary(IItem item)
            => (IsAtCapacity, Items.TryAdd(item.Id, item)) switch
            {
                (false, true) => (ItemAdded, item),
                (false, false) => (ItemAlreadyExists, item),
                _ => (InventoryAtCapacity, item)
            };

        protected virtual (InventoryAction action, IItem? item) TrySplitItemInDictionary(Guid key, int splitAmount)
        {
            if (!Items.TryGetValue(key, out var item)) return (ItemNotFound, null);
            
            var splitItem = item.SplitStack(splitAmount);
            if (splitItem == item) return (ItemStackNotSplit, item);
            
            Items.Add(splitItem.Id, splitItem);
            return (ItemStackSplit, splitItem);
        }

        protected virtual (InventoryAction action, IItem? item) TryRemoveItemFromDictionary(Guid key)
            => Items.Remove(key, out var item) 
                ? (ItemRemoved, item) 
                : (ItemNotFound, null);

        protected virtual (InventoryAction action, IEnumerable<IItem> items) TryGetItemsFromDictionaryByTag(ITag tag)
        {
            var retrievedItems = Items.Values
                .Where(item => item.ContainsTag(tag))
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>()); 
        }
        
        public abstract bool IsAtCapacity { get; }
        
        public virtual IInventoryActionResult TryAddItem(IItem item)
        {
            var (action, returnedItem) = TryAddItemToDictionary(item);
            return new InventoryActionResult(action, returnedItem);
        }
        
        public virtual IInventoryActionResult TryGetAllItems()
        {
            var (action, result) = TryGetAllItemsFromDictionary();
            return new InventoryActionResult(action, result);
        } 
        
        public virtual IInventoryActionResult TryGetItem(Guid id) 
        {
            var (action, returnedItem) = TryGetItemFromDictionary(id);
            return new InventoryActionResult(action, returnedItem);
        }
        
        public virtual IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
        {
            var (action, result) = TrySplitItemInDictionary(id, splitAmount);
            return new InventoryActionResult(action, result);
        }

        public virtual IInventoryActionResult TryRemoveItem(Guid id)
        {
            var (action, item) = TryRemoveItemFromDictionary(id);
            return new InventoryActionResult(action, item); 
        }
        
        public virtual IInventoryActionResult TryGetItemsByTag(ITag tag)
        {
            var (action, item) = TryGetItemsFromDictionaryByTag(tag);
            return new InventoryActionResult(action, item); 
        }
    }
}