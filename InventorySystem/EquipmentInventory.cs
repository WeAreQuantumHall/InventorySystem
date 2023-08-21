using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;
using static InventorySystem.Abstractions.Enums.EquipmentCategory;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem
{
    public class EquipmentInventory : Inventory
    {
        private Dictionary<EquipmentCategory, IItem?> EquipmentSlots { get; }
        
        public EquipmentInventory(string name) : base(name)
        {
            EquipmentSlots = new Dictionary<EquipmentCategory, IItem?>
            {
                {Head, null},
                {Shoulders, null},
                {Chest, null},
                {Belt, null},
                {Legs, null},
                {Feet, null},
                {MainHand, null},
                {OffHand, null}
            };
        }

        public override IInventoryActionResult TryAddItem(IItem item)
        {
            if (item.EquipmentCategory == None || !EquipmentSlots.ContainsKey(item.EquipmentCategory))
            {
                return new InventoryActionResult(NotAnValidEquipmentItem, item);
            }
            
            var hasItemInSlot = EquipmentSlots.TryGetValue(item.EquipmentCategory, out var itemInSlot);
            EquipmentSlots[item.EquipmentCategory] = item;

            return hasItemInSlot
                ? new InventoryActionResult(ItemSwapped, itemInSlot)
                : new InventoryActionResult(ItemAdded, item);
        }
    }
}