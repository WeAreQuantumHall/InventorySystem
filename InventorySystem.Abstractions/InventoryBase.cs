using System;
using InventorySystem.Abstractions.Enums;

namespace InventorySystem.Abstractions
{
    /// <inheritdoc />
    public abstract class InventoryBase : IInventory
    {
        protected InventoryBase(string name, int capacity)
        {
            Name = name;
            Id = Guid.NewGuid(); 
            Capacity = capacity < 1 ? 0 : capacity;
        } 
        
        public Guid Id { get; }
        public string Name { get; private set; }
        public int Capacity { get; }
        
        public void SetName(string name) => Name = name;
        
        public abstract int Count { get; }
        public abstract bool IsAtCapacity { get; }
        public abstract IInventoryActionResult TryAddItem(IItem item);
        public abstract IInventoryActionResult GetAllItems();
        public abstract IInventoryActionResult TryGetItem(Guid id);
        public abstract IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount);
        public abstract IInventoryActionResult TryRemoveItem(Guid id);
        public abstract IInventoryActionResult TryGetItemsByCategory(ItemCategory category);
        public abstract IInventoryActionResult SearchByTag(string tag);
    }
}