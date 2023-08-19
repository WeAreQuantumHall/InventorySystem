using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;

namespace InventorySystem.ActionResults
{
    /// <summary>
    /// Represents the result of an inventory action, along with an optional associated item.
    /// </summary>
    public class InventoryActionResult : IInventoryActionResult
    {
        /// <summary>
        /// Gets the result of the inventory action.
        /// </summary>
        public InventoryAction Result { get; }

        /// <summary>
        /// Gets the associated item with the inventory action result (if applicable).
        /// </summary>
        public IItem? Item { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryActionResult"/> class.
        /// </summary>
        /// <param name="result">The result of the inventory action.</param>
        /// <param name="item">The associated item with the inventory action result (optional).</param>
        public InventoryActionResult(InventoryAction result, IItem? item = null)
        {
            Result = result;
            Item = item;
        }
    }
}