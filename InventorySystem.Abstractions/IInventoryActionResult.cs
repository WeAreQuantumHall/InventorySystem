using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;

namespace InventorySystem.ActionResults
{
    /// <summary>
    /// Represents the result of an inventory action, along with an optional associated item.
    /// </summary>
    public interface IInventoryActionResult
    {
        /// <summary>
        /// Gets the result of the inventory action.
        /// </summary>
        InventoryAction Result { get; }

        /// <summary>
        /// Gets the associated item with the inventory action result (if applicable).
        /// </summary>
        IItem? Item { get; }
    }
}