using System;
using Xunit;

namespace InventorySystem.Tests;

public class InventoryManagerTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";
    
    [Fact]
    public void CreateInventory__ReturnsIdOfCreatedInventory()
    {
        var inventoryManager = new InventoryManager();

        var id = inventoryManager.CreateInventory(InventoryName);
        
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public void TryGetInventory_when_InventoryIsPresent__returnsTrue_and_Inventory()
    {
        var inventoryManager = new InventoryManager();
        var id = inventoryManager.CreateInventory(InventoryName);

        var isInventoryFound = inventoryManager.TryGetInventory(id, out var inventory);
        
        Assert.True(isInventoryFound);
        Assert.Equal(id, inventory.Id);
        Assert.Equal(InventoryName, inventory.Name);
    }
    
    [Fact]
    public void TryGetInventory_when_InventoryIsNotPresent__returnsFalse_and_NullInventory()
    {
        var inventoryManager = new InventoryManager();

        var isInventoryFound = inventoryManager.TryGetInventory(Guid.NewGuid(), out var inventory);
        
        Assert.False(isInventoryFound);
        Assert.Null(inventory);
    }
}