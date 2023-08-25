using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.ActionResults;
using InventorySystem.Items;
using InventorySystem.Tags;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Inventories
{
    public class EquipmentInventory : InventoryBase
    {
        public override int Count => Items.Count;
        
        /// <summary>
        /// Gets the current amount of equipment slots.
        /// </summary>
        
        /// <summary>
        ///  IsAtCapacity is not used for EquipmentInventory
        /// </summary>
        /// <returns>Always returns <see langWord="false" /></returns>
        public override bool IsAtCapacity => false;

        public EquipmentInventory(string name, IEnumerable<ITag> tags) : base(name, 0)
            => Items = tags
                .Where(EquipmentTag.IsMember)
                .ToDictionary(tag => tag.Identifier, _ => Item.Empty);
    
        public EquipmentInventory(string name) : base(name, 0) 
            => Items = EquipmentTag.Tags
                .ToDictionary(tag => tag.Identifier, _ => Item.Empty);

        protected override (InventoryAction action, IItem item) TryAddItemToDictionary(IItem item)
        {
            if (item.Stackable) return (StackableItemsNotAllowed, item);
                
            var equipmentTagsFromItem = EquipmentTag
                .GetMembers(item.TagList)
                .ToList();
                
            if (!equipmentTagsFromItem.Any()) return (ItemEquipmentTagMissing, item);

            var matchingTags = equipmentTagsFromItem
                .Where(tag => Items.ContainsKey(tag.Identifier))
                .ToList();
            
            if (!matchingTags.Any()) return (NoMatchingEquipmentSlots, item);

            var emptySlot = matchingTags
                .FirstOrDefault(tag => Items.TryGetValue(tag.Identifier, out var slotItem) && slotItem == Item.Empty);

            var slotIdentifier = emptySlot?.Identifier ?? matchingTags.First().Identifier;
            
            var itemInSlot = Items[slotIdentifier];
            Items[slotIdentifier] = item;

            return itemInSlot == Item.Empty
                ? (ItemAdded, item)
                : (ItemSwapped, itemInSlot);
        }

        protected override (InventoryAction action, IEnumerable<IItem> item) TryGetAllItemsFromDictionary()
        {
            var retrievedItems = Items.Values
                .Where(item => item != Item.Empty)
                .ToList();

            return retrievedItems.Any()
                ? (ItemsRetrieved, retrievedItems)
                : (ItemsNotFound, Enumerable.Empty<IItem>());
        }

        protected override (InventoryAction action, IItem? item) TryGetItemFromDictionary(Guid id)
        {
            var item = Items.Values
                .FirstOrDefault(item => item != Item.Empty && item.Id == id);

            return item == null
                ? (ItemNotFound, null)
                : (ItemRetrieved, item);
        }

        protected override (InventoryAction action, IItem? item) TryRemoveItemFromDictionary(Guid id)
        {
            var key = Items
                .FirstOrDefault(slot => slot.Value.Id == id)
                .Key;

            if (key == null) return (ItemNotFound, null);

            var item = Items[key];
            Items[key] = Item.Empty;
            return (ItemRemoved, item);
        }

        public override IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount) =>
            new InventoryActionResult(ItemStackNotSplit);
    }
}