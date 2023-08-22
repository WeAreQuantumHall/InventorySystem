using System;
using InventorySystem.Abstractions.Enums;

namespace InventorySystem.Abstractions
{
    public interface IInventory
    {
        /// <summary>
        /// Gets the unique identifier of the inventory.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the inventory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the capacity of the inventory. A capacity of 0 indicates a dynamically resized inventory.
        /// In cases of frequently accessed inventories, it is best to specify a capacity on creation as this
        /// will prevent the need to resize the internal dictionary during usage;  You will need to handle
        /// <see cref="InventoryAction.InventoryAtCapacity"/> results when the inventory is full.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the current count of items in the inventory
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Checks if the inventory is at maximum capacity
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if an inventory with the specified ID was found; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsAtCapacity { get; }
    
        /// <summary>
        /// Sets the name of the inventory.
        /// </summary>
        /// <param name="name">The new name for the inventory.</param>
        void SetName(string name);
        
        /// <summary>
        /// Tries to add an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the add operation.</returns>
        IInventoryActionResult TryAddItem(IItem item);

        /// <summary>
        /// Tries to get all items from the inventory.
        /// </summary>
        /// <returns>>An <see cref="IInventoryActionResult"/> indicating the result of the add operation.</returns>
        IInventoryActionResult GetAllItems(); 
        
        /// <summary>
        /// Tries to retrieve an item from the inventory by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to retrieve.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the retrieval operation.
        /// </returns>
        IInventoryActionResult TryGetItem(Guid id);

        /// <summary>
        /// Tries to split the stack of a specific item in the inventory into a new item.
        /// </summary>
        /// <param name="id">The Id of the item to split.</param>
        /// <param name="splitAmount">The amount to split from the stack.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the stack split operation.</returns>
        IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount);

        /// <summary>
        /// Tries to remove an item from the inventory.
        /// </summary>
        /// <param name="id">The Id of the item to remove.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the remove operation.</returns>
        IInventoryActionResult TryRemoveItem(Guid id);

        /// <summary>
        /// Tries to get all items in the inventory for a provided category.
        /// </summary>
        /// <param name="category">The Enum Values to search for</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the retrieve operation.</returns>
        IInventoryActionResult TryGetItemsByCategory(ItemCategory category);

        IInventoryActionResult SearchByTag(string tag);
    }
}