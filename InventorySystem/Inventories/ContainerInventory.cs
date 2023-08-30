using InventorySystem.Abstractions;

namespace InventorySystem.Inventories
{
    internal class ContainerInventory : AbstractInventory
    { 
        internal ContainerInventory(IInventoryService inventoryService, string name) 
            : base(inventoryService, name, 0) { }
    }
}