using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.ActionResults;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem
{
    
    /// <inheritdoc />
    public class Inventory : IInventory
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; private set; }

        private Dictionary<Guid, IItem> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with a specified name.
        /// </summary>
        /// <param name="name">The name of the inventory.</param>
        public Inventory(string name)
            => (Name, Id, Items) = (name, Guid.NewGuid(), new Dictionary<Guid, IItem>());

        /// <inheritdoc />
        public void SetName(string name) 
            => Name = name;

        /// <inheritdoc />
        public IInventoryActionResult TryAddItem(IItem item) 
            => item.Stackable
                ? AddStackableItem(item)
                : AddItem(item);
        
        /// <inheritdoc />
        public IInventoryActionResult TryGetItem(Guid id)
            => Items.TryGetValue(id, out var item)
                ? new InventoryActionResult(ItemRetrieved, item)
                : new InventoryActionResult(ItemNotFound, item);

        /// <inheritdoc />
        public IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
        {
            if (!Items.TryGetValue(id, out var item))
            {
                return new InventoryActionResult(ItemNotFound);
            }

            var splitItem = item.SplitStack(splitAmount);

            if (splitItem == item)
            {
                return new InventoryActionResult(ItemStackNotSplit, item);
            }

            Items.Add(splitItem.Id, splitItem);

            return new InventoryActionResult(ItemStackSplit, splitItem);
        }
        
        private InventoryActionResult AddItem(IItem item)
            => Items.TryAdd(item.Id, item)
                ? new InventoryActionResult(ItemAdded, item)
                : new InventoryActionResult(ItemAlreadyExists, item);

        private InventoryActionResult AddStackableItem(IItem item)
            => ReconcileStacks(item, out var updatedItem) switch
            {
                false when item == updatedItem => new InventoryActionResult(ItemAdded, item),
                false when item != updatedItem => new InventoryActionResult(ItemStacked, updatedItem),
                _ => new InventoryActionResult(ItemStackedAndSpilled, item)
            };

        private bool ReconcileStacks(IItem item, out IItem updatedItem)
        {
            updatedItem = item;
            var hasSpilled = false;
            
            while (TryGetSimilarStackableItem(item.Identifier, out var similarItem))
            {
                var remainingStackToAdd = similarItem.AddToStack(item.Stack);
                item.SetStack(remainingStackToAdd);

                if (item.Stack == 0)
                {
                    updatedItem = similarItem;
                    return hasSpilled;
                }
                
                hasSpilled = true;
            }
            
            Items.Add(item.Id, item);
            return hasSpilled;
        }

        private bool TryGetSimilarStackableItem(string identifier, out IItem item)
        {
            item = Items
                .FirstOrDefault(i => i.Value.Identifier == identifier && i.Value.CanBeStackedOn)
                .Value;

            return item != null;
        }
    }
}