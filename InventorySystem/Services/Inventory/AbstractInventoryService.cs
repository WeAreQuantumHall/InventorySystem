using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Services.Inventory
{
    public abstract class AbstractInventoryService : IInventoryService
    {
        public virtual (InventoryAction Action, IItem? Item) TryAddItem(
            IDictionary<Guid, IItem> items,
            IItem item,
            bool isAtCapacity)
        {
            if (isAtCapacity) return (InventoryAtCapacity, item);

            return items.TryAdd(item.Id, item)
                ? (ItemAdded, (IItem?) null)
                : (ItemAlreadyExists, (IItem?) null);
        }
        
        public virtual (InventoryAction Action, IItem? item) TryGetItem(
            IDictionary<Guid, IItem> items,
            Guid key)
            => items.TryGetValue(key, out var retrievedItem)
                ? (ItemRetrieved, retrievedItem)
                : (ItemNotFound, null);
        
        public virtual (InventoryAction Action, IEnumerable<IItem> Items) TryGetAllItems(
            IDictionary<Guid, IItem> items)
        {
            var retrievedItems = items.Values
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }

        public virtual (InventoryAction Action, IEnumerable<IItem> Items) TryGetItemsByTag(
            IDictionary<Guid, IItem> items,
            ITag tag)
        {
            var retrievedItems = items.Values
                .Where(item => item.ContainsTag(tag))
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>()); 
        }

        public virtual (InventoryAction Action, IItem? Item) TryRemoveItem(
            IDictionary<Guid, IItem> items,
            Guid key)
            => items.Remove(key, out var removedItem) 
                ? (ItemRemoved, removedItem) 
                : (ItemNotFound, null);

        public virtual (InventoryAction Action, IItem? Item) TrySplitItemStack(
            IDictionary<Guid, IItem> items,
            Guid key,
            int splitAmount)
        {
            if (!items.TryGetValue(key, out var item)) return (ItemNotFound, null);
            
            var splitItem = item.SplitStack(splitAmount);
            return splitItem == item 
                ? (ItemStackNotSplit, null)
                : (ItemStackSplit, splitItem);
        }
        
    }
}