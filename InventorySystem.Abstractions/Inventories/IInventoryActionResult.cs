using System.Collections.Generic;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Items;

namespace InventorySystem.Abstractions.Inventories
{
    /// <summary>
    /// Represents the result of an inventory action, along with an optional associated item(s).
    /// </summary>
    public interface IInventoryActionResult
    {
        /// <summary>
        /// Gets the result of the inventory action.
        /// </summary>
        InventoryAction Result { get; }

        /// <summary>
        /// Gets the associated IItem with the inventory action result (if applicable).
        /// </summary>
        IItem? Item { get; }
        
        /// <summary>
        /// Gets the associated IEnumerable of IItems with the inventory action result (if applicable)
        /// </summary>
        IEnumerable<IItem> Items { get; }
    }
}