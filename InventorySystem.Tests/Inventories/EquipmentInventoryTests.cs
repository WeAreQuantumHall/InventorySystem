using System;
using System.Collections.Generic;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Tags;
using InventorySystem.ActionResults;
using InventorySystem.Inventories;
using InventorySystem.Tags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Inventories;

public class EquipmentInventoryTests
{
    private const string InventoryName = "test-inventory-name";

    [Fact]
    public void New_WhenCategoryListNotProvided__ShouldCreateInventoryWithDefaultSlots()
    {
        var expectedNumberOfSlots = EquipmentTag.Tags.Count;
        IInventory inventory = new EquipmentInventory(InventoryName);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    } 
    
    [Fact]
    public void New_WhenCategoryListProvided__ShouldCreateInventoryWithThatAmountOfSlots()
    {
        const int expectedNumberOfSlots = 4;

        var equipmentSlots = new List<ITag>
        {
            EquipmentTag.Belt, 
            EquipmentTag.Head, 
            EquipmentTag.Chest, 
            EquipmentTag.MainHand
        };
        IInventory inventory = new EquipmentInventory(InventoryName, equipmentSlots);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    }

    [Fact]
    public void IsAtCapacity__ReturnsFalse()
    {
        IInventory inventory = new EquipmentInventory(InventoryName);

        Assert.False(inventory.IsAtCapacity);
    }

    [Fact]
    public void TryAddItem_when_ItemIsStackable__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToAddMock = MockItemData.GetItemMock(true, 1, 99);
        var expectedAddItemActionResult = new InventoryActionResult(StackableItemsNotAllowed, itemToAddMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName);
        var addItemActionResult = inventory.TryAddItem(itemToAddMock.Object);
        
        Assert.Equivalent(expectedAddItemActionResult, addItemActionResult);
    }

    [Fact]
    public void TryAddItem_when_ItemDoesNotHaveEquipmentTag__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock.Setup(tagList => tagList.Tags)
            .Returns(new List<ITag>());
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        
        
        var expectedAddItemActionResult = new InventoryActionResult(ItemEquipmentTagMissing, itemToAddMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName);
        var addItemActionResult = inventory.TryAddItem(itemToAddMock.Object);
        
        Assert.Equivalent(expectedAddItemActionResult, addItemActionResult);
    }

    [Fact]
    public void TryAddItem_when_ItemDoesNotContainMatchingSlot__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        
        var expectedAddItemActionResult = new InventoryActionResult(NoMatchingEquipmentSlots, itemToAddMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName, new []{EquipmentTag.Legs});
        var addItemActionResult = inventory.TryAddItem(itemToAddMock.Object);
        
        Assert.Equivalent(expectedAddItemActionResult, addItemActionResult);
        
    }

    [Fact]
    public void TryAddItem_when_ItemHasMatchingEmptySlots__ReturnsExpectedActionResult_and_ItemAdded()
    {
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        
        var expectedAddItemActionResult = new InventoryActionResult(ItemAdded, itemToAddMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName);
        var addItemActionResult = inventory.TryAddItem(itemToAddMock.Object);
        var getAddedItemActionResult = inventory.TryGetItem(itemToAddMock.Object.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddItemActionResult, addItemActionResult),
            () => Assert.Equal(ItemRetrieved, getAddedItemActionResult.Result));
    }

    [Fact]
    public void TryAddItem_when_ItemHasNoMatchingEmptySlots__ReturnsExpectedActionResult_and_SwappedItem()
    {
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var existingItemToSwapMock = MockItemData.GetItemMock(false, 1, 1);
        existingItemToSwapMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);

        var expectedAddItemActionResult = new InventoryActionResult(ItemSwapped, existingItemToSwapMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName, new []{EquipmentTag.Head, EquipmentTag.Legs});
        inventory.TryAddItem(existingItemToSwapMock.Object);
        var addItemActionResult = inventory.TryAddItem(itemToAddMock.Object);
        var getSwappedItemActionResult = inventory.TryGetItem(existingItemToSwapMock.Object.Id);
        var getAddedItemActionResult = inventory.TryGetItem(itemToAddMock.Object.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddItemActionResult, addItemActionResult),
            () => Assert.Equal(ItemNotFound, getSwappedItemActionResult.Result),
            () => Assert.Equal(ItemRetrieved, getAddedItemActionResult.Result));
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
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        itemToAddMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);

        var expectedGetAllItemsActionResult = new InventoryActionResult(ItemsRetrieved, new[] {itemToAddMock.Object});
        
        IInventory inventory = new EquipmentInventory(InventoryName);
        inventory.TryAddItem(itemToAddMock.Object);
        
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
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var itemToGetMock = MockItemData.GetItemMock(false, 1, 1);
        itemToGetMock
            .Setup(item => item.Id)
            .Returns(itemToGetId);
        itemToGetMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToGetMock.Object);
    
        IInventory inventory = new EquipmentInventory(InventoryName);
        inventory.TryAddItem(itemToGetMock.Object);
        var getActionResult = inventory.TryGetItem(itemToGetId);
        
        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryRemoveItem_when_ItemDoesNotExist_ReturnsExpectedActionResult_WithoutItem()
    {
        var expectedRemoveActionResult = new InventoryActionResult(ItemNotFound);

        var inventory = new EquipmentInventory(InventoryName);
        var removeActionResult = inventory.TryRemoveItem(Guid.NewGuid());
        
        Assert.Equivalent(expectedRemoveActionResult, removeActionResult);
    }
    
    [Fact]
    public void TryRemoveItem_when_ItemExists_ReturnsExpectedActionResult_with_ItemAndSetsSlotToEmptyItem()
    {
        var itemToRemoveId = Guid.NewGuid();
        var tagList = new List<ITag> {EquipmentTag.Head};
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        var itemToRemoveMock = MockItemData.GetItemMock(false, 1, 1);
        itemToRemoveMock
            .Setup(item => item.TagList)
            .Returns(tagListMock.Object);
        itemToRemoveMock
            .Setup(item => item.Id)
            .Returns(itemToRemoveId);
        var expectedRemoveItemActionResult = new InventoryActionResult(ItemRemoved, itemToRemoveMock.Object);
        
        IInventory inventory = new EquipmentInventory(InventoryName);
        inventory.TryAddItem(itemToRemoveMock.Object);

        var removeItemActionResult = inventory.TryRemoveItem(itemToRemoveId);
        var getItemActionResult = inventory.TryGetItem(itemToRemoveId);
        
        Assert.Multiple(
            () => Assert.Equivalent(expectedRemoveItemActionResult, removeItemActionResult),
            () => Assert.Equal(ItemNotFound, getItemActionResult.Result));
    }
    
    [Fact]
    public void TrySplitItemStack__ReturnsItemStackNotSplit()
    {
        var expectedSplitActionResult = new InventoryActionResult(ItemStackNotSplit);

        var inventory = new EquipmentInventory(InventoryName);
        var splitActionResult = inventory.TrySplitItemStack(It.IsAny<Guid>(), It.IsAny<int>());
        
        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }
}