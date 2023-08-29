using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Inventories;
using InventorySystem.Tags;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Inventories;

public class EquipmentInventoryTests
{
    private const string InventoryName = "test-inventory-name";
    private readonly Mock<IInventoryService> _inventoryServiceMock = new Mock<IInventoryService>(); 
    
    [Constructor]
    public void Creating_an_inventory_without_providing_categories_will_create_an_inventory_with_the_default_items()
    {
        var expectedNumberOfItems = EquipmentTag.Tags.Count;
        IInventory inventory = new EquipmentInventory(_inventoryServiceMock.Object, InventoryName);

        Assert.Equal(expectedNumberOfItems, inventory.Count);
    } 
    
    [Constructor]
    public void Creating_an_inventory_with_categories_provided_will_create_an_inventory_with_those_items() 
    {
        const int expectedNumberOfItems = 4;
        var equipmentSlots = new List<ITag>
        {
            EquipmentTag.Belt, 
            EquipmentTag.Head, 
            EquipmentTag.Chest, 
            EquipmentTag.MainHand
        };
        
        IInventory inventory = new EquipmentInventory(_inventoryServiceMock.Object, InventoryName, equipmentSlots);
        
        Assert.Equal(expectedNumberOfItems, inventory.Count);
    }

    [HappyPath]
    public void The_inventory_is_never_at_capacity()
    {
        IInventory inventory = new EquipmentInventory(_inventoryServiceMock.Object, InventoryName);
    
        Assert.False(inventory.IsAtCapacity);
    }
    
    
    
    // [UnhappyPath]
    // public void 
    //
    //
    // [Fact]
    // public void GetAllItems_when_NoItemsHaveBeenAdded__ShouldReturnExpectedActionResult()
    // {
    //     var expectedGetAllItemsActionResult = new InventoryActionResult(ItemsNotFound); 
    //
    //     IInventory inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     var getAllItemsActionResult = inventory.TryGetAllItems();
    //     
    //     Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult);
    // }
    //
    //
    // [Fact]
    // public void GetAllItems_when_ItemsHaveBeenAdded__ShouldReturnExpectedActionResult()
    // {
    //     var tagList = new List<ITag> {EquipmentTag.Head};
    //     var tagListMock = new Mock<ITagList>();
    //     tagListMock
    //         .Setup(tl => tl.Tags)
    //         .Returns(tagList);
    //     var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
    //     itemToAddMock
    //         .Setup(item => item.TagList)
    //         .Returns(tagListMock.Object);
    //
    //     var expectedGetAllItemsActionResult = new InventoryActionResult(ItemsRetrieved, new[] {itemToAddMock.Object});
    //     
    //     IInventory inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     inventory.TryAddItem(itemToAddMock.Object);
    //     
    //     var getAllItemsActionResult = inventory.TryGetAllItems();
    //     
    //     Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult);
    // }
    //
    // [Fact]
    // public void TryGetItem_when_ItemDoesNotExist__ShouldReturnExpectedActionResult()
    // {
    //     var notFoundId = Guid.NewGuid();
    //     var expectedGetActionResult = new InventoryActionResult(ItemNotFound);
    //
    //     IInventory inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     var getActionResult = inventory.TryGetItem(notFoundId);
    //     
    //     Assert.Equivalent(expectedGetActionResult, getActionResult);
    // }
    //
    // [Fact]
    // public void TryGetItem_when_ItemExists__ShouldReturnExpectedActionResult_with_Item()
    // {
    //     var itemToGetId = Guid.NewGuid();
    //     var tagList = new List<ITag> {EquipmentTag.Head};
    //     var tagListMock = new Mock<ITagList>();
    //     tagListMock
    //         .Setup(tl => tl.Tags)
    //         .Returns(tagList);
    //     var itemToGetMock = MockItemData.GetItemMock(false, 1, 1);
    //     itemToGetMock
    //         .Setup(item => item.Id)
    //         .Returns(itemToGetId);
    //     itemToGetMock
    //         .Setup(item => item.TagList)
    //         .Returns(tagListMock.Object);
    //     var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToGetMock.Object);
    //
    //     IInventory inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     inventory.TryAddItem(itemToGetMock.Object);
    //     var getActionResult = inventory.TryGetItem(itemToGetId);
    //     
    //     Assert.Equivalent(expectedGetActionResult, getActionResult);
    // }
    //
    // [Fact]
    // public void TryRemoveItem_when_ItemDoesNotExist_ReturnsExpectedActionResult_WithoutItem()
    // {
    //     var expectedRemoveActionResult = new InventoryActionResult(ItemNotFound);
    //
    //     var inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     var removeActionResult = inventory.TryRemoveItem(Guid.NewGuid());
    //     
    //     Assert.Equivalent(expectedRemoveActionResult, removeActionResult);
    // }
    //
    // [Fact]
    // public void TryRemoveItem_when_ItemExists_ReturnsExpectedActionResult_with_ItemAndSetsSlotToEmptyItem()
    // {
    //     var itemToRemoveId = Guid.NewGuid();
    //     var tagList = new List<ITag> {EquipmentTag.Head};
    //     var tagListMock = new Mock<ITagList>();
    //     tagListMock
    //         .Setup(tl => tl.Tags)
    //         .Returns(tagList);
    //     var itemToRemoveMock = MockItemData.GetItemMock(false, 1, 1);
    //     itemToRemoveMock
    //         .Setup(item => item.TagList)
    //         .Returns(tagListMock.Object);
    //     itemToRemoveMock
    //         .Setup(item => item.Id)
    //         .Returns(itemToRemoveId);
    //     var expectedRemoveItemActionResult = new InventoryActionResult(ItemRemoved, itemToRemoveMock.Object);
    //     
    //     IInventory inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     inventory.TryAddItem(itemToRemoveMock.Object);
    //
    //     var removeItemActionResult = inventory.TryRemoveItem(itemToRemoveId);
    //     var getItemActionResult = inventory.TryGetItem(itemToRemoveId);
    //     
    //     Assert.Multiple(
    //         () => Assert.Equivalent(expectedRemoveItemActionResult, removeItemActionResult),
    //         () => Assert.Equal(ItemNotFound, getItemActionResult.Result));
    // }
    //
    // [Fact]
    // public void TrySplitItemStack__ReturnsItemStackNotSplit()
    // {
    //     var expectedSplitActionResult = new InventoryActionResult(StackableItemsNotAllowed);
    //
    //     var inventory = new EquipmentInventory(_inventoryServiceMock, InventoryName);
    //     var splitActionResult = inventory.TrySplitItemStack(It.IsAny<Guid>(), It.IsAny<int>());
    //     
    //     Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    // }
}