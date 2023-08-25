using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.ActionResults;

namespace InventorySystem.Inventories
{
    public class Inventory : InventoryBase
    {
        public override bool IsAtCapacity => Capacity > 0 && Count == Capacity;

        public Inventory(string name, int capacity = 0) : base(name, capacity) {}
        
        public override IInventoryActionResult TryAddItem(IItem item)
        {
            var(action, returnedItem) = item.Stackable
                ? TryAddStackableItem(item)
                : TryAddItemToDictionary(item);
            
            return new InventoryActionResult(action, returnedItem);
        }
    }
}