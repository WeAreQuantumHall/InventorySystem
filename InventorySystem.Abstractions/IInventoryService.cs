using System;
using System.Collections.Generic;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Abstractions
{
    public interface IInventoryService
    {
        (InventoryAction Action, IItem? Item) TryAddItem(
            IDictionary<Guid, IItem> items,
            IItem item,
            bool isAtCapacity);
        (InventoryAction Action, IItem? item) TryGetItem(
            IDictionary<Guid, IItem> items,
            Guid key);
        (InventoryAction Action, IEnumerable<IItem> Items) TryGetAllItems(
            IDictionary<Guid, IItem> items);
        (InventoryAction Action, IItem? Item) TryRemoveItem(
            IDictionary<Guid, IItem> items,
            Guid key);
        (InventoryAction Action, IItem? Item) TrySplitItemStack(
            IDictionary<Guid, IItem> items,
            Guid key,
            int splitAmount);
        (InventoryAction Action, IEnumerable<IItem> Items) TryGetItemsByTag(
            IDictionary<Guid, IItem> items,
            ITag tag);
    }
}