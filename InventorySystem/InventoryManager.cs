﻿using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;

namespace InventorySystem
{
    /// <summary>
    /// Manages a collection of inventories.
    /// </summary>
    public class InventoryManager
    {
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();

        /// <summary>
        /// Creates a new inventory with the specified name and adds it to the manager.
        /// </summary>
        /// <param name="name">The name of the new inventory.</param>
        /// <returns>The unique identifier of the created inventory.</returns>
        public Guid CreateInventory(string name)
        {
            var inventory = new Inventory(name);
            _inventories.Add(inventory.Id, inventory);
            return inventory.Id;
        }

        /// <summary>
        /// Tries to retrieve an inventory by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the inventory.</param>
        /// <param name="inventory">The retrieved inventory, if found.</param>
        /// <returns>
        /// <see langword="true" /> if an inventory with the specified ID was found; otherwise, <see langword="false" />.</returns>
        public bool TryGetInventory(Guid id, out IInventory inventory)
            => _inventories.TryGetValue(id, out inventory);
    }
}