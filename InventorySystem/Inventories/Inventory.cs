using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Inventories
{
    
    /// <inheritdoc />
    public class Inventory : InventoryBase
    {
        private Dictionary<Guid, IItem> Items { get; } = new Dictionary<Guid, IItem>();

        private Dictionary<Type, Func<Enum, IEnumerable<IItem>>> SearchCategories { get; }

        /// <inheritdoc />
        public override int Count => Items.Count;

        public override bool IsAtCapacity => Capacity > 0 && Count == Capacity;

        //private override Type _searchCategoryType = typeof(EquipmentCategory); 


        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class with a specified name and capacity.
        /// </summary>
        /// <param name="name">The name of the inventory.</param>
        /// <param name="capacity">The capacity of the inventory.</param>
        public Inventory(string name, int capacity = 0) : base(name, capacity)
        {
            if (capacity > 0) Items.EnsureCapacity(capacity);
            SearchCategories = new Dictionary<Type, Func<Enum, IEnumerable<IItem>>>
            {
                {typeof(EquipmentCategory), category => GetByEquipmentCategory((EquipmentCategory) category)}
            };
        }

        /// <inheritdoc />
        public override IInventoryActionResult TryAddItem(IItem item) 
            => item.Stackable
                ? TryAddStackableItem(item)
                : TryAddItemToDictionary(item);

        /// <inheritdoc />
        public override IInventoryActionResult TryGetItem(Guid id)
            => Items.TryGetValue(id, out var item)
                ? new InventoryActionResult(ItemRetrieved, item)
                : new InventoryActionResult(ItemNotFound, item);

        /// <inheritdoc />
        public override IInventoryActionResult TryGetItemByCategory<TEnum>(TEnum category)
        {
            var categoryType = typeof(TEnum);

            if (!SearchCategories.TryGetValue(categoryType, out var func))
            {
                return new InventoryActionResult(SearchCategoryInvalid);
            }
                
            var items = func(category).ToArray();
            return items.Length > 0
                ? new InventoryActionResult(ItemsRetrieved, items)
                : new InventoryActionResult(ItemsNotFound);
        }

        private IEnumerable<IItem> GetByEquipmentCategory(EquipmentCategory category)
            => Items
                .Where(kvp => kvp.Value.EquipmentCategory == category)
                .Select(kvp => kvp.Value);


        /// <inheritdoc />
        public override IInventoryActionResult GetAllItems()
        {
          var items = Items.Select(kvp => kvp.Value).ToArray();
          return items.Length == 0
              ? new InventoryActionResult(ItemsNotFound)
              : new InventoryActionResult(ItemsRetrieved, items);
        } 

        
        /// <inheritdoc />
        public override IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
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
        public override IInventoryActionResult TryRemoveItem(Guid id)
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
        
        /// <summary>
        /// Tries to retrieve the first item in the inventory with matching identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the item to be found.</param>
        /// <param name="item">Outputs the item if it was found.</param>
        /// <returns>
        /// <see langword="true"/>when item found; otherwise <see langword="false" />
        /// </returns>
        private bool TryGetSimilarStackableItem(string identifier, out IItem item)
        {
            item = Items
                .FirstOrDefault(i => i.Value.Identifier == identifier && i.Value.CanBeStackedOn)
                .Value;

            return item != null;
        }
    }
}