using System;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Abstractions.Inventories
{
    public interface IInventory
    {
        Guid Id { get; }
        string Name { get; }
        int Capacity { get; }
        int Count { get; }
        public bool IsAtCapacity { get; }
        void SetName(string name);
        IInventoryActionResult TryAddItem(IItem item);
        IInventoryActionResult TryGetItem(Guid id);
        IInventoryActionResult TryGetAllItems();
        IInventoryActionResult TrySplitItemStack(Guid id, int splitAmount);
        IInventoryActionResult TryRemoveItem(Guid id);
        IInventoryActionResult TryGetItemsByTag(ITag tag);
    }
}