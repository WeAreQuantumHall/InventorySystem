using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.ActionResults;

namespace InventorySystem.Inventories
{
    public abstract class AbstractInventory : IInventory
    {
        protected AbstractInventory(IInventoryService inventoryService, string name, int capacity)
        {
            Name = name;
            Id = Guid.NewGuid(); 
            Capacity = capacity < 1 ? 0 : capacity;
            Items.EnsureCapacity(capacity);
            InventoryService = inventoryService;
        }

        private IInventoryService InventoryService { get; }
        protected Dictionary<Guid, IItem> Items { get; set; } = new Dictionary<Guid, IItem>();
        public Guid Id { get; }
        public string Name { get; private set; }
        public int Capacity { get; }
        
        public virtual int Count => Items.Count;
        public virtual bool IsAtCapacity => Capacity > 0 && Count == Capacity;
        public virtual void SetName(string name) => Name = name;

        public virtual IInventoryActionResult TryAddItem(IItem item)
        {
            var (action, returnedItem) = InventoryService.TryAddItem(Items, item, IsAtCapacity); 
            return new InventoryActionResult(action, returnedItem);
        }
 
        public virtual IInventoryActionResult TryGetItem(Guid id)
        {
            var (action, returnedItem) = InventoryService.TryGetItem(Items, id);
            return new InventoryActionResult(action, returnedItem);
        }
        
        public virtual IInventoryActionResult TryGetAllItems()
        {
            var (action, result) = InventoryService.TryGetAllItems(Items);
            return new InventoryActionResult(action, result);
        }
        
        public virtual IInventoryActionResult TryGetItemsByTag(ITag tag)
        {
            var (action, item) = InventoryService.TryGetItemsByTag(Items, tag);
            return new InventoryActionResult(action, item); 
        }
 
        public virtual IInventoryActionResult TryRemoveItem(Guid id)
        {
            var (action, item) = InventoryService.TryRemoveItem(Items, id);
            return new InventoryActionResult(action, item); 
        }

        public virtual IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount)
        {
            var (action, result) = InventoryService.TrySplitItemStack(Items, id, splitAmount);
            return new InventoryActionResult(action, result);
        }
    }
}