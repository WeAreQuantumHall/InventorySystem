// using System;
// using System.Collections.Generic;
// using System.Linq;
// using InventorySystem.Abstractions;
// using InventorySystem.Abstractions.Enums;
// using InventorySystem.ActionResults;
// using static InventorySystem.Abstractions.Enums.EquipmentCategory;
// using static InventorySystem.Abstractions.Enums.InventoryAction;
//
// namespace InventorySystem.Inventories
// {
//     public class EquipmentInventory : InventoryBase
//     {
//         private Dictionary<EquipmentCategory, IItem?> EquipmentSlots { get; }
//         
//         /// <summary>
//         /// Gets the current amount of equipment slots.
//         /// </summary>
//         public override int Count => EquipmentSlots.Count;
//
//         /// <summary>
//         ///  IsAtCapacity is not used for EquipmentInventory
//         /// </summary>
//         /// <returns>Always returns <see langWord="false" /></returns>
//         public override bool IsAtCapacity => false;
//
//         public EquipmentInventory(string name, IEnumerable<EquipmentCategory> categories) : base(name, 0)
//         {
//             EquipmentSlots = categories.ToDictionary(c => c, _ => (IItem?) null);
//         }
//         
//         public EquipmentInventory(string name) : base(name, 0)
//         {
//             EquipmentSlots = new Dictionary<EquipmentCategory, IItem?>
//             {
//                 {Head, null},
//                 {Shoulders, null},
//                 {Chest, null},
//                 {Belt, null},
//                 {Legs, null},
//                 {Feet, null},
//                 {MainHand, null},
//                 {OffHand, null}
//             };
//         }
//         
//         /// <inheritdoc />
//         public override IInventoryActionResult TryAddItem(IItem item)
//         {
//             if (item.ItemCategory != ItemCategory.Equipment || !EquipmentSlots.ContainsKey(item.Subcategory))
//             {
//                 return new InventoryActionResult(NotAnValidEquipmentItem, item);
//             }
//             
//             var hasItemInSlot = EquipmentSlots.TryGetValue(item.ItemCategory, out var itemInSlot);
//             EquipmentSlots[item.ItemCategory] = item;
//
//             return hasItemInSlot
//                 ? new InventoryActionResult(ItemSwapped, itemInSlot)
//                 : new InventoryActionResult(ItemAdded, item);
//         }
//
//         /// <inheritdoc />
//         public override IInventoryActionResult GetAllItems()
//         {
//             throw new NotImplementedException();
//         }
//         
//         /// <inheritdoc />
//         public override IInventoryActionResult TryGetItem(Guid id)
//         {
//             throw new NotImplementedException();
//         }
//
//         /// <inheritdoc />
//         public override IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
//         {
//             throw new NotImplementedException();
//         }
//         
//         /// <inheritdoc />
//         public override IInventoryActionResult TryRemoveItem(Guid id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public override IInventoryActionResult TryGetItemsByCategory<TEnum>(TEnum category)
//         {
//             // if(typeof(TEnum) != typeof(ItemCategory))
//             // {
//             //     return new InventoryActionResult(,);
//             // } 
//             //
//             throw new NotImplementedException();
//         }
//     }
// }