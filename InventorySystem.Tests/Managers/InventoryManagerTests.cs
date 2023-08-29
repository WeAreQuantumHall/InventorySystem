using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Items;
using InventorySystem.ActionResults;
using InventorySystem.Managers;
using InventorySystem.Tests.AttributeTags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Managers;

public class InventoryManagerTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";
    private readonly Mock<IInventoryService> _inventoryServiceMock = new Mock<IInventoryService>(); 
    
    [HappyPath]
    public void Creating_a_new_inventory_returns_the_id_of_the_new_inventory()
    {
        var inventoryManager = new InventoryManager();

        var id = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);
        
        Assert.NotEqual(Guid.Empty, id);
    }

    [HappyPath]
    public void Can_get_an_inventory_when_the_inventory_is_present_and_the_inventory_is_provided()
    {
        var inventoryManager = new InventoryManager();
        var id = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);

        var isInventoryFound = inventoryManager.TryGetInventory(id, out var inventory);
        
        Assert.True(isInventoryFound);
        Assert.Equal(id, inventory.Id);
        Assert.Equal(InventoryName, inventory.Name);
    }
    
    [UnhappyPath]
    public void Cannot_get_an_inventory_when_the_inventory_is_not_present_and_the_inventory_is_not_provided()
    {
        var inventoryManager = new InventoryManager();

        var isInventoryFound = inventoryManager.TryGetInventory(Guid.NewGuid(), out var inventory);
        
        Assert.False(isInventoryFound);
        Assert.Null(inventory);
    }

    [HappyPath]
    public void Creating_multiple_inventories_add_multiple_inventories()
    {
        var inventoryManager = new InventoryManager();
        var firstInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object,InventoryName, 0);
        var secondInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);
        
        var isFirstInventoryFound = inventoryManager.TryGetInventory(firstInventoryId, out var firstInventory);
        var isSecondInventoryFound = inventoryManager.TryGetInventory(secondInventoryId, out var secondInventory);
        
        Assert.Multiple(
            () => Assert.True(isFirstInventoryFound),
            () => Assert.True(isSecondInventoryFound),
            () => Assert.NotEqual(firstInventory, secondInventory));
    }
    
    [HappyPath]
    public void Can_move_item_between_inventories_when_both_inventories_and_the_item_are_present()
    {
        var itemToMoveId = Guid.NewGuid();
        var itemToMoveMock = MockItemData.GetItemMock(false, 1, 1);

        _inventoryServiceMock.Setup(service => service.TryRemoveItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                itemToMoveId))
            .Returns((ItemRemoved, itemToMoveMock.Object));
        
        _inventoryServiceMock.Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(), 
                itemToMoveMock.Object, false))
            .Returns((ItemAdded, null));
        var expectedItemMoveActionResult = new InventoryActionResult(ItemMovedBetweenInventories);
        
        var inventoryManager = new InventoryManager();
        var sourceInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);
        var targetInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);

        var itemMoveActionResult = inventoryManager.MoveItemBetweenInventories(
            sourceInventoryId,
            targetInventoryId,
            itemToMoveId);
        
        Assert.Equivalent(expectedItemMoveActionResult, itemMoveActionResult);
    }
    
    [UnhappyPath]
    public void Cannot_move_the_item_between_inventories_when_the_source_inventory_is_not_present()
    {
        var itemToMoveId = Guid.NewGuid();
        var sourceInventoryId = Guid.NewGuid();
        var expectedItemMoveActionEvent = new InventoryActionResult(SourceInventoryNotFound);
        
        var inventoryManager = new InventoryManager();
        var targetInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);

        var itemMoveActionResult = inventoryManager.MoveItemBetweenInventories(
            sourceInventoryId, 
            targetInventoryId, 
            itemToMoveId);
        
        Assert.Equivalent(expectedItemMoveActionEvent, itemMoveActionResult);
    }
    
    [UnhappyPath]
    public void Cannot_move_the_item_between_inventories_when_the_target_inventory_is_not_present()
    {
        var itemToMoveId = Guid.NewGuid();
        var targetInventoryId = Guid.NewGuid();
        var expectedItemMoveActionResult = new InventoryActionResult(TargetInventoryNotFound);

        var inventoryManager = new InventoryManager();
        var sourceInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);

        var itemMoveActionResult = inventoryManager.MoveItemBetweenInventories(
            sourceInventoryId,
            targetInventoryId,
            itemToMoveId);
        
        Assert.Equivalent(expectedItemMoveActionResult, itemMoveActionResult);
    }
    
    [UnhappyPath]
    public void Cannot_move_item_between_inventories_when_the_item_is_not_present_in_the_source_inventory()
    {
        var itemToMoveId = Guid.NewGuid();
        
        _inventoryServiceMock.Setup(service => service.TryRemoveItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                itemToMoveId))
            .Returns((ItemNotFound, null));
        var expectedItemMoveActionResult = new InventoryActionResult(ItemNotFound);
        
        var inventoryManager = new InventoryManager();
        var sourceInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);
        var targetInventoryId = inventoryManager.CreateInventory(_inventoryServiceMock.Object, InventoryName, 0);

        var itemMovedActionResult = inventoryManager.MoveItemBetweenInventories(
            sourceInventoryId, 
            targetInventoryId, 
            itemToMoveId);
        
        Assert.Equivalent(expectedItemMoveActionResult, itemMovedActionResult);
    }
}