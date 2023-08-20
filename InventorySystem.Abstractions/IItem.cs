using System;

namespace InventorySystem.Abstractions
{
    /// <summary>
    /// Represents an item that can be part of an inventory.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Gets the unique identifier of the item.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the identifier of the item.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets whether the item can be stacked.
        /// </summary>
        bool Stackable { get; }

        /// <summary>
        /// Gets the current stack amount of the item.
        /// </summary>
        int Stack { get; }

        /// <summary>
        /// Gets the maximum stack amount of the item.
        /// </summary>
        int MaxStack { get; }

        /// <summary>
        /// Gets whether the item can be stacked on top of another item.
        /// </summary>
        bool CanBeStackedOn { get; }

        /// <summary>
        /// Sets the stack amount of the item up to a maximum specified by MaxStack.
        /// </summary>
        /// <param name="amount">The new stack amount.</param>
        /// <returns>The remaining stack which could not be added.</returns>
        int AddToStack(int amount);

        /// <summary>
        /// Splits the item stack into new items.
        /// </summary>
        /// <param name="splitAmount">The stack amount of the new item.</param>
        /// <returns>The new item with requested stack amount.</returns>
        IItem SplitStack(int splitAmount);
    }
}