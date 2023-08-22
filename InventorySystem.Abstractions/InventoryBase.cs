using System;

namespace InventorySystem.Abstractions
{
    public abstract class InventoryBase : IInventory
    {
        /// <inheritdoc />
        public Guid Id { get; }
        
        /// <inheritdoc />
        public string Name { get; private set; }
        
        /// <inheritdoc />
        public int Capacity { get; }
        
        protected InventoryBase(string name, int capacity) 
            => (Name, Id, Capacity) = (name, Guid.NewGuid(), capacity < 1 ? 0 : capacity);
        
        /// <inheritdoc />
        public void SetName(string name) => Name = name;
        
        /// <inheritdoc />
        public abstract int Count { get; }
        
        /// <inheritdoc />
        public abstract bool IsAtCapacity { get; }
        
        /// <inheritdoc />
        public abstract IInventoryActionResult TryAddItem(IItem item);
        
        /// <inheritdoc />
        public abstract IInventoryActionResult GetAllItems();
        
        /// <inheritdoc />
        public abstract IInventoryActionResult TryGetItem(Guid id);
        
        /// <inheritdoc />
        public abstract IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount);
        
        /// <inheritdoc />
        public abstract IInventoryActionResult TryRemoveItem(Guid id);

        public abstract IInventoryActionResult TryGetItemByCategory<TEnum>(TEnum category) where TEnum : Enum;
    }
}