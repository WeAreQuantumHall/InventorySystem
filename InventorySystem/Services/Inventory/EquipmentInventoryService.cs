using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Items;
using InventorySystem.Tags;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Services.Inventory
{
    public class EquipmentInventoryService : AbstractInventoryService
    {
        public override (InventoryAction Action, IItem? Item) TryAddItem(
            IDictionary<Guid, IItem> items,
            IItem item,
            bool isAtCapacity)
        {
            if (item.ItemStack != null) return (StackableItemsNotAllowed, item);
                
            var equipmentTagsFromItem = EquipmentTag
                .GetMembers(item.TagList)
                .ToList();
                
            if (!equipmentTagsFromItem.Any()) return (ItemEquipmentTagMissing, item);

            var matchingTags = equipmentTagsFromItem
                .Where(tag => items.ContainsKey(tag.Identifier))
                .ToList();
            
            if (!matchingTags.Any()) return (NoMatchingEquipmentSlots, item);

            IItem? slotItem = null;
            var emptySlot = matchingTags
                .FirstOrDefault(tag => items.TryGetValue(tag.Identifier, out slotItem) && slotItem == IItem.Empty);

            if (emptySlot == null)
            {
                var identifier = matchingTags.First().Identifier;
                items[identifier] = item;
                return (ItemSwapped, slotItem);
            }
            
            items[emptySlot.Identifier] = item;
            return (ItemAdded, null);

        }

        public override (InventoryAction Action, IItem? item) TryGetItem(
            IDictionary<Guid, IItem> items,
            Guid id) 
            => items.TryGetValue(id, out var item) && item != IItem.Empty
                ? (ItemRetrieved, item) 
                : (ItemNotFound, null);

        public override (InventoryAction Action, IEnumerable<IItem> Items) TryGetAllItems(
            IDictionary<Guid, IItem> items)
        {
            var retrievedItems = items.Values
                .Where(item => item != IItem.Empty)
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }
        
        
        public override (InventoryAction Action, IItem? Item) TryRemoveItem(
            IDictionary<Guid, IItem> items, 
            Guid id)
        {
            if (!items.TryGetValue(id, out var itemInSlot) || itemInSlot == IItem.Empty)
                return (ItemNotFound, null);

            items[id] = IItem.Empty;
            return (ItemRemoved, itemInSlot);
        }

        public override (InventoryAction Action, IItem? Item) TrySplitItemStack(
            IDictionary<Guid, IItem> items,
            Guid key,
            int splitAmount)
            => (StackableItemsNotAllowed, null);
    }
}