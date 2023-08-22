using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;

namespace InventorySystem.ActionResults
{
    /// <inheritdoc />
    public class InventoryActionResult : IInventoryActionResult
    {
        /// <inheritdoc />
        public InventoryAction Result { get; }

        /// <inheritdoc />
        public IItem? Item { get; }

        /// <inheritdoc />
        public IEnumerable<IItem>? Items { get; }

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

        public InventoryActionResult(InventoryAction result, IEnumerable<IItem> items)
        {
            Result = result;
            Items = items;
        }
    }
}