using System;
using System.Collections.Generic;

namespace InventorySystem
{
    public class InventoryManager
    {
        private readonly Dictionary<Guid, Inventory> _inventories;

        public InventoryManager()
        {
            _inventories = new Dictionary<Guid, Inventory>();
        }

        public Guid CreateInventory(string name)
        {
            var inventory = new Inventory(name);
            _inventories.Add(inventory.Id, inventory);
            return inventory.Id;
        }

        public bool TryGetInventory(Guid id, out Inventory inventory)
            => _inventories.TryGetValue(id, out inventory);
    }
}