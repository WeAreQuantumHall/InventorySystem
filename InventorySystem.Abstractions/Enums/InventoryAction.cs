namespace InventorySystem.Abstractions.Enums
{
    /// <summary>
    /// Represents the possible actions that can occur during inventory operations.
    /// </summary>
    public enum InventoryAction
    {
        /// <summary>
        /// An item was successfully added to the inventory.
        /// </summary>
        ItemAdded,

        /// <summary>
        /// An attempt was made to add an item that already exists in the inventory.
        /// </summary>
        ItemAlreadyExists,

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
        /// An item was not successfully removed from the inventory.
        /// </summary>
        ItemNotRemoved,

        /// <summary>
        /// An item was successfully stacked onto another item in the inventory.
        /// </summary>
        ItemStacked,

        /// <summary>
        /// An item was stacked onto another item, and the stack spilled, requiring an additional item to be added.
        /// </summary>
        ItemStackedAndSpilled,

        /// <summary>
        /// An item's stack was successfully split into two separate stacks in the inventory.
        /// </summary>
        ItemStackSplit,
        
        /// <summary>
        /// An item's stack was not successfully split into two separate stacks in the inventory.
        /// </summary>
        ItemStackNotSplit
    }
}