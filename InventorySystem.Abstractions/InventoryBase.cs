using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Abstractions
{
    /// <inheritdoc />
    public abstract class InventoryBase : IInventory
    {
        protected InventoryBase(string name, int capacity)
        {
            Name = name;
            Id = Guid.NewGuid(); 
            Capacity = capacity < 1 ? 0 : capacity;
        }
        
        protected Dictionary<Guid, IItem> Items { get; } = new Dictionary<Guid, IItem>();
        
        public Guid Id { get; }
        public string Name { get; private set; }
        public int Capacity { get; }
        
        public void SetName(string name) => Name = name;

        /// <summary>
        /// Tries to add a stackable item to <see cref="Items"/> dictionary.
        /// </summary>
        /// <param name="item">The item to be added to the dictionary.</param>
        /// <param name="items">The list of times to be stacked or added on.</param>
        /// <param name="inventoryKeyPredicate"></param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the retrieval operation.</returns>
        protected virtual (InventoryAction action, IItem? item) TryAddStackableItem(IItem item)
        {
            var hasSpilled = false;
 
            foreach (var similarItem in TryGetSimilarStackableItems(item.Identifier))
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
        
        protected virtual (InventoryAction action, IEnumerable<IItem> item)TryGetItemsFromDictionaryByCategory(
                ItemCategory category)
        {
            var retrievedItems = Items.Values
                .Where(item => item.ItemCategory == category)
                .ToList();
   
            
            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }

        protected virtual (InventoryAction action, IEnumerable<IItem> item) TryGetAllItemsFromDictionary()
        {
            var retrievedItems = Items.Values
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }
        protected virtual IEnumerable<IItem> TryGetSimilarStackableItems(
            string identifier)  
            => Items.Values
                .Where(i => i.Identifier == identifier && i.CanBeStackedOn);

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

        protected virtual (InventoryAction action, IItem? item) TryRemoveItemFromDictionary<TKeyType>(
            Dictionary<TKeyType, IItem> items,
            TKeyType key) where TKeyType: IComparable
            => items.Remove(key, out var item) 
                ? (ItemRemoved, item) 
                : (ItemNotRemoved, null);

        protected virtual (InventoryAction aaction, IEnumerable<IItem> items) TryGetItemsFromDictionaryByTag<TKeyType>(
            Dictionary<TKeyType, IItem> items,
            string tag) where TKeyType: IComparable
        {
            var retrievedItems = items.Values
                .Where(item => item.ContainsTag(tag))
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>()); 
        }
        
        
        public abstract int Count { get; }
        public abstract bool IsAtCapacity { get; }
        public abstract IInventoryActionResult TryAddItem(IItem item);
        public abstract IInventoryActionResult TryGetAllItems();
        public abstract IInventoryActionResult TryGetItem(Guid key);
        public abstract IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount);
        public abstract IInventoryActionResult TryRemoveItem(Guid id);
        public abstract IInventoryActionResult TryGetItemsByCategory(ItemCategory category);
        public abstract IInventoryActionResult TryGetItemsByTag(string tag);
    }
}