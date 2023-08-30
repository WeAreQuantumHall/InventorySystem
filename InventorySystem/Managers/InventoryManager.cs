using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.ActionResults;
using InventorySystem.Inventories;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Managers
{
    public class InventoryManager
    {
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();

        public bool TryGetInventory(Guid id, out IInventory inventory)
            => _inventories.TryGetValue(id, out inventory);
        
        public Guid CreateInventory(IInventoryService inventoryService, string name, int capacity)
        {
            var inventory = new ContainerInventory(inventoryService, name);
            _inventories.Add(inventory.Id, inventory);
            return inventory.Id;
        }

       public IInventoryActionResult MoveItemBetweenInventories(Guid sourceInventoryId, Guid targetInventoryId, Guid itemToMoveId)
        {
            if (!TryGetInventory(sourceInventoryId, out var sourceInventory))
                return new InventoryActionResult(SourceInventoryNotFound);
            if (!TryGetInventory(targetInventoryId, out var targetInventory))
                return new InventoryActionResult(TargetInventoryNotFound);
            
            var removedItemActionResult = sourceInventory.TryRemoveItem(itemToMoveId);
            if (removedItemActionResult.Result != ItemRemoved)
                return new InventoryActionResult(ItemNotFound);

            targetInventory.TryAddItem(removedItemActionResult.Item!);

            return new InventoryActionResult(ItemMovedBetweenInventories);
        }
    }
}