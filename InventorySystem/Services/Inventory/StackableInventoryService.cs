using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Items;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Services.Inventory
{
    public class StackableInventoryService : AbstractInventoryService
    {
        public override (InventoryAction Action, IItem? Item) TryAddItem(
            IDictionary<Guid, IItem> items,
            IItem item,
            bool isAtCapacity)
        {
            if (!item.Stackable) return base.TryAddItem(items, item, isAtCapacity);
            
            var hasStacked = false;

            foreach (var similarItem in TryGetSimilarItems(items, item.Name))
            {
                var remainingStack = similarItem.AddToStack(item.Stack);

                if (remainingStack == 0) return (ItemStacked, null);

                item.SetStack(remainingStack);
                hasStacked = true;
            }

            if (isAtCapacity) return (InventoryAtCapacity, item);
            
            return items.TryAdd(item.Id, item)
                ? (hasStacked ? ItemStackedAndAdded : ItemAdded, (IItem?) null)
                : (ItemAlreadyExists, null);
        }

        
        private static IEnumerable<IItem> TryGetSimilarItems(IDictionary<Guid, IItem> items, string name)
            => items.Values.Where(i => i.Name == name && i.CanBeStackedOn);
    }
}