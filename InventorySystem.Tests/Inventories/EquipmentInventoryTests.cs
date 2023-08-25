using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;
using InventorySystem.Inventories;
using InventorySystem.Tags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;
using static InventorySystem.Tags.EquipmentTag;

namespace InventorySystem.Tests.Inventories;

public class EquipmentInventoryTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";

    [Fact]
    public void New_WhenCategoryListNotProvided__ShouldCreateInventoryWithDefaultSlots()
    {
        var expectedNumberOfSlots = TagUtils.EquipmentTags.Tags.Count;
        IInventory inventory = new EquipmentInventory(InventoryName);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    } 
    
    [Fact]
    public void New_WhenCategoryListProvided__ShouldCreateInventoryWithThatAmountOfSlots()
    {
        const int expectedNumberOfSlots = 4;

        var equipmentSlots = new List<string> {Belt, Head, Chest, MainHand};
        IInventory inventory = new EquipmentInventory(InventoryName, equipmentSlots);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    }

    [Fact]
    public void IsAddCapacity__ReturnsFalse()
    {
        IInventory inventory = new EquipmentInventory(InventoryName);

        Assert.False(inventory.IsAtCapacity);
    }
    
    [Fact]
    public void TryAddItem_when_ItemIsIncorrectCategory__ShouldReturnExpectedActionResult_and_NotAddItem()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        itemMock
            .Setup(item => item.ItemCategory)
            .Returns(ItemCategory.Food);

        var expectedAddActionResult = new InventoryActionResult(InvalidItemCategory, itemMock.Object); 

        IInventory inventory = new EquipmentInventory(InventoryName);
        var addActionResult = inventory.TryAddItem(itemMock.Object); 
        
        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }
    
    [Fact]
    public void TryAddItem_when_NoMatchingSlotsFound__ShouldReturnExpectedActionResult_and_NotAddItem()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        itemMock
            .Setup(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(false);
   
        var expectedAddActionResult = new InventoryActionResult(NoMatchingSlots, itemMock.Object); 

        IInventory inventory = new EquipmentInventory(InventoryName, new [] {Head, Chest});
        var addActionResult = inventory.TryAddItem(itemMock.Object); 
        
        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }
    
    [Fact]
    public void TryAddItem_when_NoEmptySlots__ShouldReturnExpectedActionResult_with_SwappedItem()
    {
        var itemInSlotMock = MockItemData.GetItemMock(false, 1, 1);
        itemInSlotMock
            .SetupSequence(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true)
            .Returns(false);
        
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .SetupSequence(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true)
            .Returns(false);
        
        var expectedAddActionResult = new InventoryActionResult(ItemSwapped, itemInSlotMock.Object); 

        IInventory inventory = new EquipmentInventory(InventoryName, new [] {Head, Chest});
        inventory.TryAddItem(itemInSlotMock.Object); 
        
        var addActionResult = inventory.TryAddItem(itemToAddMock.Object); 
        
        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }
    
    [Fact]
    public void TryAddItem_when_AreEmptySlots__ShouldReturnExpectedActionResult_with_AddedItem()
    {
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .SetupSequence(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true);
        
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToAddMock.Object); 

        IInventory inventory = new EquipmentInventory(InventoryName, new [] {Head, Chest});
        var addActionResult = inventory.TryAddItem(itemToAddMock.Object); 
        
        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }

    [Fact]
    public void GetAllItems_when_NoItemsHaveBeenAdded__ShouldReturnExpectedActionResult()
    {
        var expectedGetAllItemsActionResult = new InventoryActionResult(ItemsNotFound); 

        IInventory inventory = new EquipmentInventory(InventoryName);
        var getAllItemsActionResult = inventory.TryGetAllItems();
        
        Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult);
    }

    [Fact]
    public void GetAllItems_when_ItemsHaveBeenAdded__ShouldReturnExpectedActionResult()
    {
        var firstItemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        firstItemToAddMock
            .SetupSequence(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true);
        var secondItemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        secondItemToAddMock
            .SetupSequence(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true);

        var expectedGetAllItemsActionResult = new InventoryActionResult(ItemsNotFound);

        IInventory inventory = new EquipmentInventory(InventoryName);
        var getAllItemsActionResult = inventory.TryGetAllItems();

        Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult);
    }
    
    [Fact]
    public void TryGetItem_when_ItemDoesNotExist__ShouldReturnExpectedActionResult()
    {
        var notFoundId = Guid.NewGuid();
        var expectedGetActionResult = new InventoryActionResult(ItemNotFound);

        IInventory inventory = new EquipmentInventory(InventoryName);
        var getActionResult = inventory.TryGetItem(notFoundId);
        
        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryGetItem_when_ItemExists__ShouldReturnExpectedActionResult_with_Item()
    {
        var itemToGetId = Guid.NewGuid();
        var itemToGetMock = MockItemData.GetItemMock(false, 1, 1);
        itemToGetMock
            .Setup(item => item.Id)
            .Returns(itemToGetId);
        itemToGetMock
            .Setup(item => item.ContainsTag(It.IsAny<string>()))
            .Returns(true);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToGetMock.Object);

        IInventory inventory = new EquipmentInventory(InventoryName);
        inventory.TryAddItem(itemToGetMock.Object);
        var getActionResult = inventory.TryGetItem(itemToGetId);
        
        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }
}