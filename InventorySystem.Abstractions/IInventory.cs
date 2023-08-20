using System;

namespace InventorySystem.Abstractions
{
    public interface IInventory
    {
        /// <summary>
        /// Gets the unique identifier of the inventory.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets or sets the name of the inventory.
        /// </summary>
        string Name { get; }

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
        /// Tries to retrieve an item from the inventory by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to retrieve.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the retrieval operation.</returns>
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
    }
}