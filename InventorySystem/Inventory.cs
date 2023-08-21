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

        /// <inheritdoc />
        public int Capacity { get; }

        /// <inheritdoc />
        public int Count => Items.Count;

        public bool IsAtCapacity => Capacity > 0 && Count == Capacity;

        private Dictionary<Guid, IItem> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with a specified name.
        /// </summary>
        /// <param name="name">The name of the inventory.</param>
        public Inventory(string name)
            => (Name, Id, Items, Capacity) = (name, Guid.NewGuid(), new Dictionary<Guid, IItem>(), 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with a specified name and capacity.
        /// </summary>
        /// <param name="name">The name of the inventory.</param>
        /// <param name="capacity">The capacity of the inventory.</param>
        public Inventory(string name, int capacity)
        {
            (Name, Id, Capacity) = (name, Guid.NewGuid(), capacity < 1 ? 0 : capacity);
            
            Items = Capacity == 0
                ? new Dictionary<Guid, IItem>()
                : new Dictionary<Guid, IItem>(capacity);
        }
         
        /// <inheritdoc />
        public void SetName(string name) 
            => Name = name;

        /// <inheritdoc />
        public IInventoryActionResult TryAddItem(IItem item) 
            => item.Stackable
                ? TryAddStackableItem(item)
                : TryAddItemToDictionary(item);
        
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

        /// <inheritdoc />
        public IInventoryActionResult TryRemoveItem(Guid id)
        {
            return Items.Remove(id, out var removedItem)
                ? new InventoryActionResult(ItemRemoved, removedItem) 
                : new InventoryActionResult(ItemNotRemoved); 
        }

        /// <summary>
        /// Tries to add an item to <see cref="Items"/> dictionary.
        /// </summary>
        /// <param name="item">The item to be added to the dictionary.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the add operation.
        /// </returns>
        private IInventoryActionResult TryAddItemToDictionary(IItem item) => 
            (IsAtCapacity, Items.TryAdd(item.Id, item)) switch
            {
                (false, true) => new InventoryActionResult(ItemAdded, item),
                (false, false) => new InventoryActionResult(ItemAlreadyExists, item),
                _ => new InventoryActionResult(InventoryAtCapacity, item)
            };

        /// <summary>
        /// Tries to add a stackable item to <see cref="Items"/> dictionary.
        /// </summary>
        /// <param name="item">The item to be added to the dictionary.</param>
        /// <returns>An <see cref="IInventoryActionResult"/> indicating the result of the retrieval operation.</returns>
        private IInventoryActionResult TryAddStackableItem(IItem item)
        {
            var hasSpilled = false;
            
            while (TryGetSimilarStackableItem(item.Identifier, out var similarItem))
            {
                var remainingStack = similarItem.AddToStack(item.Stack);
                item.AddToStack(remainingStack);
                
                if (remainingStack == 0)
                {
                    return hasSpilled
                        ? new InventoryActionResult(ItemStackedAndSpilled, similarItem)
                        : new InventoryActionResult(ItemStacked, similarItem);
                }
                
                hasSpilled = true;
            }
            
            return TryAddItemToDictionary(item);
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