using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;
using InventorySystem.Tags;
using static InventorySystem.Abstractions.Enums.InventoryAction;
using static InventorySystem.Abstractions.Enums.ItemCategory;

namespace InventorySystem.Inventories
{
    public class EquipmentInventory : InventoryBase
    {
        private Dictionary<string, IItem?> EquipmentSlots { get; }
        
        /// <summary>
        /// Gets the current amount of equipment slots.
        /// </summary>
        public override int Count => EquipmentSlots.Count;

        /// <summary>
        ///  IsAtCapacity is not used for EquipmentInventory
        /// </summary>
        /// <returns>Always returns <see langWord="false" /></returns>
        public override bool IsAtCapacity => false;

        public EquipmentInventory(string name, IEnumerable<string> categories) : base(name, 0)
        {
            EquipmentSlots = categories.ToDictionary(c => c, _ => (IItem?) null);
        }
        
        public EquipmentInventory(string name) : base(name, 0)
        {
            EquipmentSlots = TagUtils.EquipmentTags.Tags.ToDictionary(tag => tag, _ => (IItem?) null);
        }
        
        /// <inheritdoc />
        public override IInventoryActionResult TryAddItem(IItem item)
        {
            if (item.ItemCategory != Equipment) return new InventoryActionResult(InvalidItemCategory, item); 
            
            var matchingSlots = EquipmentSlots.Keys
                .Where(item.ContainsTag)
                .ToList();
            
            if (!matchingSlots.Any()) return new InventoryActionResult(NoMatchingSlots, item); 

            var emptySlot = EquipmentSlots
                .FirstOrDefault(slot => matchingSlots.Contains(slot.Key) && slot.Value == null)
                .Key;
                
            if (string.IsNullOrEmpty(emptySlot))
            {
                EquipmentSlots.TryGetValue(matchingSlots.First(), out var itemInSlot);
                EquipmentSlots[matchingSlots.First()] = item;

                return new InventoryActionResult(ItemSwapped, itemInSlot);
            }
            
            EquipmentSlots[emptySlot] = item;
            return new InventoryActionResult(ItemAdded, item);
        }

        /// <inheritdoc />
        public override IInventoryActionResult TryGetAllItems()
        {
            var items = new List<IItem>(EquipmentSlots.Values.Where(item => item != null)!);

            return items.Any()
                ? new InventoryActionResult(ItemsRetrieved, items)
                : new InventoryActionResult(ItemsNotFound);
        }

        /// <inheritdoc />
        public override IInventoryActionResult TryGetItem(Guid id)
        {
            var item = EquipmentSlots.Values.FirstOrDefault(item => item != null && item.Id == id);
                
            return item == null
                ? new InventoryActionResult(ItemNotFound, item)   
                : new InventoryActionResult(ItemRetrieved, item);
        }
        
        /// <inheritdoc />
        public override IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
        {
            throw new NotImplementedException();
        }
        
        /// <inheritdoc />
        public override IInventoryActionResult TryRemoveItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public override IInventoryActionResult TryGetItemsByCategory(ItemCategory category)
        {
            throw new NotImplementedException();
        }

        public override IInventoryActionResult TryGetItemsByTag(string tag)
        {
            throw new NotImplementedException();
        }
    }
}