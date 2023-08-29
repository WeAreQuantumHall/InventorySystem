using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;

namespace InventorySystem.Inventories
{
    internal class EquipmentInventory : AbstractInventory
    {
        public override bool IsAtCapacity => false;

        internal EquipmentInventory(IInventoryService inventoryService, string name, IEnumerable<ITag> tags) 
            : base(inventoryService, name, 0)
            => Items = tags
                .Where(EquipmentTag.IsMember)
                .ToDictionary(tag => tag.Identifier, _ => IItem.Empty);

        internal EquipmentInventory(IInventoryService inventoryService, string name)
            : base(inventoryService, name, 0)
            => Items = EquipmentTag.Tags
                .ToDictionary(tag => tag.Identifier, _ => IItem.Empty);
    }
}