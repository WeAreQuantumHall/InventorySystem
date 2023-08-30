using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;

namespace InventorySystem.ActionResults
{
    internal class InventoryActionResult : IInventoryActionResult
    {
        public InventoryActionResult(InventoryAction result, IItem? item = null)
            => (Result, Item) = (result, item);

        public InventoryActionResult(InventoryAction result, IEnumerable<IItem> items)
            => (Result, Items) = (result, items);
        
        public InventoryAction Result { get; }
        public IItem? Item { get; }
        public IEnumerable<IItem> Items { get; } = Enumerable.Empty<IItem>();
    }
}