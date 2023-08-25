using System;
using InventorySystem.Abstractions.Items;
using InventorySystem.Managers;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Managers;

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

    [Fact]
    public void CreateMultipleTimes__AddsMultipleInventories()
    {
        var inventoryManager = new InventoryManager();
        var firstInventoryId = inventoryManager.CreateInventory(InventoryName);
        var secondInventoryId = inventoryManager.CreateInventory(InventoryName);
        
        var isFirstInventoryFound = inventoryManager.TryGetInventory(firstInventoryId, out var firstInventory);
        var isSecondInventoryFound = inventoryManager.TryGetInventory(secondInventoryId, out var secondInventory);
        
        Assert.Multiple(
            () => Assert.True(isFirstInventoryFound),
            () => Assert.True(isSecondInventoryFound),
            () => Assert.NotEqual(firstInventory, secondInventory));
    }

    [Fact]
    public void MoveItemBetweenInventories_whenSourceInventoryNotFound__ReturnsFalse()
    {
        var inventoryManager = new InventoryManager();
        var sourceInventoryId = Guid.NewGuid();
        var itemToMoveId = Guid.NewGuid();
        var targetInventoryId = inventoryManager.CreateInventory(InventoryName);

        var hasItemBeenMoved =
            inventoryManager.MoveItemBetweenInventories(sourceInventoryId, targetInventoryId, itemToMoveId);
        
        Assert.False(hasItemBeenMoved);
    }
    
    [Fact]
    public void MoveItemBetweenInventories_whenTargetInventoryNotFound__ReturnsFalse()
    {
        var inventoryManager = new InventoryManager();
        var targetInventoryId = Guid.NewGuid();
        var itemToMoveId = Guid.NewGuid();
        
        var sourceInventoryId = inventoryManager.CreateInventory(InventoryName);

        var hasItemBeenMoved =
            inventoryManager.MoveItemBetweenInventories(sourceInventoryId, targetInventoryId, itemToMoveId);
        
        Assert.False(hasItemBeenMoved);
    }
    
    [Fact]
    public void MoveItemBetweenInventories_whenItemToMoveNotFound__ReturnsFalse()
    {
        var inventoryManager = new InventoryManager();
        var itemToMoveId = Guid.NewGuid();

        var sourceInventoryId = inventoryManager.CreateInventory(InventoryName);
        var targetInventoryId = inventoryManager.CreateInventory(InventoryName);

        var hasItemBeenMoved =
            inventoryManager.MoveItemBetweenInventories(sourceInventoryId, targetInventoryId, itemToMoveId);
        
        Assert.False(hasItemBeenMoved);
    }

    [Fact]
    public void MoveItemBetweenInventories_with_NonStackableItem__ReturnsTrue()
    {
        var inventoryManager = new InventoryManager();
        var itemToMoveId = Guid.NewGuid();
        var itemToMove = new Mock<IItem>();
        itemToMove
            .SetupGet(item => item.Id)
            .Returns(itemToMoveId);
        itemToMove
            .SetupGet(item => item.Stackable)
            .Returns(false);
        
        var sourceInventoryId = inventoryManager.CreateInventory(InventoryName);
        var targetInventoryId = inventoryManager.CreateInventory(InventoryName);

        inventoryManager.TryGetInventory(sourceInventoryId, out var sourceInventory);
        sourceInventory.TryAddItem(itemToMove.Object);
        
        var hasItemBeenMoved =
            inventoryManager.MoveItemBetweenInventories(sourceInventoryId, targetInventoryId, itemToMoveId);
        
        Assert.True(hasItemBeenMoved);
    }
    
    [Fact]
    public void MoveItemBetweenInventories_with_StackableItem__ReturnsTrue()
    {
        var inventoryManager = new InventoryManager();
        var itemToMoveId = Guid.NewGuid();
        var itemToMove = new Mock<IItem>();
        itemToMove
            .SetupGet(item => item.Id)
            .Returns(itemToMoveId);
        itemToMove
            .SetupGet(item => item.Stackable)
            .Returns(true);
        itemToMove
            .SetupGet(item => item.Stack)
            .Returns(1);
        itemToMove
            .SetupGet(item => item.MaxStack)
            .Returns(10);
        
        var sourceInventoryId = inventoryManager.CreateInventory(InventoryName);
        var targetInventoryId = inventoryManager.CreateInventory(InventoryName);

        inventoryManager.TryGetInventory(sourceInventoryId, out var sourceInventory);
        sourceInventory.TryAddItem(itemToMove.Object);
        
        var hasItemBeenMoved =
            inventoryManager.MoveItemBetweenInventories(sourceInventoryId, targetInventoryId, itemToMoveId);
        
        Assert.True(hasItemBeenMoved);
    }
}