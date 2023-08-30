using System.Diagnostics.CodeAnalysis;

namespace InventorySystem.Abstractions.Enums
{
    /// <summary>
    /// Represents the possible actions that can occur during inventory operations.
    /// </summary>
    public enum InventoryAction
    {
        /// <summary>
        /// Default value for the enum, should not be used.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        None,
        
        /// <summary>
        /// An item was successfully added to the inventory.
        /// </summary>
        ItemAdded,

        /// <summary>
        /// An attempt was made to add an item that already exists in the inventory.
        /// </summary>
        ItemAlreadyExists,
        
        /// <summary>
        /// An attempt was made to add an item that already exists in the inventory.
        /// </summary>
        InventoryAtCapacity,

        /// <summary>
        /// An item was successfully retrieved from the inventory.
        /// </summary>
        ItemRetrieved,

        /// <summary>
        /// The requested item was not found in the inventory.
        /// </summary>
        ItemNotFound,
        
        /// <summary>
        /// An item was successfully removed from the inventory.
        /// </summary>
        ItemRemoved,
        
        /// <summary>
        /// An item was successfully stacked onto another item in the inventory.
        /// </summary>
        ItemStacked,

        /// <summary>
        /// An item was stacked onto another item, and the stack spilled, requiring an additional item to be added.
        /// </summary>
        ItemStackedAndAdded,

        /// <summary>
        /// An item's stack was successfully split into two separate stacks in the inventory.
        /// </summary>
        ItemStackSplit,
        
        /// <summary>
        /// An item's stack was not successfully split into two separate stacks in the inventory.
        /// </summary>
        ItemStackNotSplit,
        
        /// <summary>
        /// An item is not an equipment item and cannot be added to the inventory
        /// </summary>
        ItemEquipmentTagMissing,
        
        /// <summary>
        /// An item was swapped with another item in the stack
        /// </summary>
        ItemSwapped,
        
        /// <summary>
        /// A collection of items was retrieved. 
        /// </summary>
        ItemsRetrieved,
        
        /// <summary>
        /// No items were found in the inventory
        /// </summary>
        ItemsNotFound,
        
        /// <summary>
        /// There were no matching equipment slots on the item
        /// </summary>
        NoMatchingEquipmentSlots,
        
        /// <summary>
        /// The inventory does not allow items with an ItemStack
        /// </summary>
        StackableItemsNotAllowed,
        
        /// <summary>
        /// The source inventory id provided does not match to an inventory
        /// </summary>
        SourceInventoryNotFound,
        
        /// <summary>
        /// The target inventory id provided does not match to an inventory
        /// </summary>
        TargetInventoryNotFound,
        
        /// <summary>
        /// The item has successfully been moved from the source inventory to the target inventory
        /// </summary>
        ItemMovedBetweenInventories
        
    }
}