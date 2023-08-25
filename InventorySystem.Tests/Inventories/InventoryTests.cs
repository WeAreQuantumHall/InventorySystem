using System;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.ActionResults;
using InventorySystem.Inventories;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Inventories;

public class InventoryTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";


    [Fact]
    public void New_without_Capacity__CreatesCorrectlyPopulatedValues()
    {
        const int expectedCapacity = 0;

        IInventory inventory = new Inventory(InventoryName);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, inventory.Id),
            () => Assert.Equal(InventoryName, inventory.Name),
            () => Assert.Equal(expectedCapacity, inventory.Capacity));
    }

    [Fact]
    public void New_with_Capacity__CreatesCorrectlyPopulatedValues()
    {
        const int capacity = 10;
        IInventory inventory = new Inventory(InventoryName, capacity);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, inventory.Id),
            () => Assert.Equal(InventoryName, inventory.Name),
            () => Assert.Equal(capacity, inventory.Capacity));
    }

    [Fact]
    public void SetName__CorrectlySetsName()
    {
        const string newInventoryName = "TEST_NAME_NEW";
        IInventory inventory = new Inventory(InventoryName);

        inventory.SetName(newInventoryName);

        Assert.Equal(newInventoryName, inventory.Name);
    }

    [Fact]
    public void TryAddItem_when_NotStackable_And_InventoryAtMaxCapacity__ReturnsExpectedActionResult_an_NotAddItem()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(InventoryAtCapacity, itemToAddMock.Object);
        
        IInventory inventory = new Inventory(InventoryName, 1);
        inventory.TryAddItem(itemMock.Object);
        var addActionResult = inventory.TryAddItem(itemToAddMock.Object);

        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }
    
    [Fact]
    public void TryAddItem_when_Stackable_And_InventoryAtMaxCapacity__ReturnsExpectedActionResult_an_NotAddItem()
    {
        var itemAtCloseToCapacityMock = MockItemData.GetItemMock(true, 8, 10);
        itemAtCloseToCapacityMock
            .SetupSequence(item => item.Stack)
            .Returns(10);
        itemAtCloseToCapacityMock
            .SetupSequence(item => item.CanBeStackedOn)
            .Returns(true)
            .Returns(false);
        itemAtCloseToCapacityMock
            .Setup(item => item.AddToStack(10))
            .Returns(8);

        var itemToStackMock = MockItemData.GetItemMock(true, 10, 30);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(8)
            .Returns(8);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(InventoryAtCapacity, itemToStackMock.Object);
        
        IInventory inventory = new Inventory(InventoryName, 1);
        inventory.TryAddItem(itemAtCloseToCapacityMock.Object);
        var addActionResult = inventory.TryAddItem(itemToStackMock.Object);

        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }

    [Fact]
    public void TryAddItem_when_NotStackable_and_NotPresent__ReturnsExpectedActionResult_and_AddsItem()
    {
        const int expectedCount = 1;
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(ItemAdded, itemMock.Object);

        var addActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedCount, inventory.Count));
    }
    
    [Fact]
    public void TryAddItem_when_NotStackable_and_Present__ReturnsExpectedActionResult_and_NotAddItem()
    {
        const int expectedCount = 1;
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(ItemAlreadyExists, itemMock.Object);
        
        inventory.TryAddItem(itemMock.Object);
        var addActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedCount, inventory.Count));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarItemNotPresent__ReturnsExpectedActionResult_and_AddItem()
    {
        const int expectedCount = 1;
        var itemMock = MockItemData.GetItemMock(true, 10, 30);
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(ItemAdded, itemMock.Object);

        var addActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedCount, inventory.Count));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_ShouldBeStacked__ReturnExpectedActionResult_and_NotAddItem()
    {
        const int expectedCount = 1;
        const int expectedStack = 20;
        var itemToBeStackedOnMock = MockItemData.GetItemMock(true, 10, 30);
        var itemToStackMock = MockItemData.GetItemMock(true, 10, 30);
        itemToBeStackedOnMock
            .Setup(item => item.Stack)
            .Returns(20);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(0);
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(ItemStacked, itemToBeStackedOnMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(itemToBeStackedOnMock.Object);

        var addActionResult = inventory.TryAddItem(itemToStackMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedStack, itemToBeStackedOnMock.Object.Stack),
            () => Assert.Equal(expectedCount, inventory.Count));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarMaxStackItemPresent__ShouldReturnExpectedActionResult_and_AddItem()
    {
        const int expectedStack = 10;
        const int expectedCount = 2;

        var maxCapacityItemMock = MockItemData.GetItemMock(true, 10, 10);
        var itemToBeStackedMock = MockItemData.GetItemMock(true, 10, 30);
        
        IInventoryActionResult expectedAddActionResult 
            = new InventoryActionResult(ItemAdded, itemToBeStackedMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(maxCapacityItemMock.Object);

        var addActionResult = inventory.TryAddItem(itemToBeStackedMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedStack, itemToBeStackedMock.Object.Stack),
            () => Assert.Equal(expectedCount, inventory.Count));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarItemPresent_and_ShouldHaveSpilled__ShouldReturnExpectedActionResult_and_AddItem()
    {
        const int expectedStack = 8;
        const int expectedCount = 2;

        var itemAtCloseToCapacityMock = MockItemData.GetItemMock(true, 8, 10);
        itemAtCloseToCapacityMock
            .SetupSequence(item => item.Stack)
            .Returns(10);
        itemAtCloseToCapacityMock
            .SetupSequence(item => item.CanBeStackedOn)
            .Returns(true)
            .Returns(false);
        itemAtCloseToCapacityMock
            .Setup(item => item.AddToStack(10))
            .Returns(8);

        var itemToStackMock = MockItemData.GetItemMock(true, 10, 30);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(8)
            .Returns(8);
        
        IInventoryActionResult expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToStackMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(itemAtCloseToCapacityMock.Object);

        var addActionResult = inventory.TryAddItem(itemToStackMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(itemAtCloseToCapacityMock.Object.MaxStack, itemAtCloseToCapacityMock.Object.Stack),
            () => Assert.Equal(expectedStack, itemToStackMock.Object.Stack),
            () => Assert.Equal(expectedCount, inventory.Count));
    }
    
    [Fact]
    public void TryGetItem_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedGetActionResult = new InventoryActionResult(ItemNotFound);

        var getActionResult = inventory.TryGetItem(id);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryGetItem_when_ItemIsPresent__ReturnsExpectedActionResult_with_Item()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemMock.Object);

        inventory.TryAddItem(itemMock.Object);
        var getActionResult = inventory.TryGetItem(itemMock.Object.Id);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryGetItemByCategory_when_NoItemsFound__ReturnsExpectedActionResult_with_NoItems()
    {
        var firstItemMock = MockItemData.GetItemMock(false, 1, 1);
        
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedGetByCategoryActionResult = new InventoryActionResult(ItemsNotFound);
        
        inventory.TryAddItem(firstItemMock.Object);
        var getByCategoryActionResult = inventory.TryGetItemsByCategory(ItemCategory.Food);
        
        Assert.Equivalent(expectedGetByCategoryActionResult, getByCategoryActionResult);
    }
    
    [Fact]
    public void TryGetItemByCategory_when_ItemsFound__ReturnsExpectedActionResult_with_ItemEnumerable()
    {
        var firstItemMock = MockItemData.GetItemMock(false, 1, 1);
        var secondItemMock = MockItemData.GetItemMock(false, 1, 1);
        secondItemMock
            .Setup(item => item.ItemCategory)
            .Returns(ItemCategory.Food);
        
        IInventoryActionResult expectedGetByCategoryActionResult = new InventoryActionResult(ItemsRetrieved, new [] 
        {
            secondItemMock.Object
        });
        
        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(firstItemMock.Object);
        inventory.TryAddItem(secondItemMock.Object);
        
        var getByCategoryActionResult = inventory.TryGetItemsByCategory(ItemCategory.Food);
        
        Assert.Equivalent(expectedGetByCategoryActionResult, getByCategoryActionResult);
    }

    [Fact]
    public void GetAllItems_when_NoItemsPresent__ReturnsExpectedActionResult_with_NoItems()
    {
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedGetAllItemsActionResult = new InventoryActionResult(ItemsNotFound);

        var getAllItemsActionResult = inventory.TryGetAllItems();
        
        Assert.Multiple(
            () => Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult),
            () => Assert.Empty(getAllItemsActionResult.Items));
    }
    
    [Fact]
    public void GetAllItems_when_ItemsPresent__ReturnsExpectedActionResult_with_Items()
    {
        const int expectedCount = 2;
        var firstItemMock = MockItemData.GetItemMock(false, 1, 1);
        var secondItemMock = MockItemData.GetItemMock(false, 1, 1);
        IInventoryActionResult expectedGetAllItemsActionResult = new InventoryActionResult(ItemsRetrieved, new []
        {
            firstItemMock.Object,secondItemMock.Object
        });
        
        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(firstItemMock.Object);
        inventory.TryAddItem(secondItemMock.Object);
        
        var getAllItemsActionResult = inventory.TryGetAllItems();
        
        Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult);Assert.Multiple(
            () => Assert.Equivalent(expectedGetAllItemsActionResult, getAllItemsActionResult),
            () => Assert.NotNull(getAllItemsActionResult.Items),
            () => Assert.Equal(expectedCount, getAllItemsActionResult.Items!.Count()));
    }

    [Fact]
    public void TrySplitItemStack_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedSplitActionResult = new InventoryActionResult(ItemNotFound);

        var splitActionResult = inventory.TrySplitItemStack(id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItemStack_when_ItemIsPresent_and_NotStackable__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToSplitMock = MockItemData.GetItemMock(false, 1, 1);
        itemToSplitMock
            .Setup(item => item.SplitStack(1))
            .Returns(itemToSplitMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedSplitActionResult 
            = new InventoryActionResult(ItemStackNotSplit, itemToSplitMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);
        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void
        TrySplitItemStack_when_ItemIsPresent_and_StackCannotBeSplit__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToSplitMock = MockItemData.GetItemMock(true, 1, 2);
        itemToSplitMock
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(itemToSplitMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedSplitActionResult 
            = new InventoryActionResult(ItemStackNotSplit, itemToSplitMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);

        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItem_when_ItemIsPresent_andStackCanBeSplit__ReturnsExpectedActionResult_and_AddsNewItem()
    {
        var splitItemMock = MockItemData.GetItemMock(true, 2, 2);
        var itemToSplitMock = MockItemData.GetItemMock(true, 1, 2);
        itemToSplitMock
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(splitItemMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        IInventoryActionResult expectedSplitActionResult
            = new InventoryActionResult(ItemStackSplit, splitItemMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);

        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void IsAtCapacity_when_DictionaryIsAtCapacity__ReturnsTrue()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        
        IInventory inventory = new Inventory(InventoryName, 1);
        inventory.TryAddItem(itemMock.Object);
        
        Assert.True(inventory.IsAtCapacity);
    }
    
    [Fact]
    public void IsAtCapacity_when_DictionaryIsNotAtCapacity__ReturnsFalse()
    {
        var itemMock = MockItemData.GetItemMock(false, 1, 1);
        
        IInventory inventory = new Inventory(InventoryName, 2);
        inventory.TryAddItem(itemMock.Object);
        
        Assert.False(inventory.IsAtCapacity);
    }

    [Fact]
    public void SearchByTag_when_DictionaryContainItemsWithTag__ReturnsExpectedActionResult_with_Items()
    {
        var itemToFindMock = MockItemData.GetItemMock(false, 1, 1);
        const string itemTagToFind = "ITEM_TAG_2";
        itemToFindMock
            .Setup(item => item.ContainsTag(itemTagToFind))
            .Returns(true);
            
        var notFoundItemMock = MockItemData.GetItemMock(false, 1, 1);
        notFoundItemMock
            .Setup(item => item.ContainsTag(itemTagToFind))
            .Returns(false);


        var expectedSearchByTagActionResult = new InventoryActionResult(ItemsRetrieved, new[] {itemToFindMock.Object});

        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(itemToFindMock.Object);
        inventory.TryAddItem(notFoundItemMock.Object);

        var searchByTagActionResult = inventory.TryGetItemsByTag(itemTagToFind);
        
        Assert.Equivalent(expectedSearchByTagActionResult, searchByTagActionResult);
    }
    
    [Fact]
    public void SearchByTag_when_DictionaryDoesNotContainItemsWithTag__ReturnsExpectedActionResult_without_Items()
    {
        var firstItemMock = MockItemData.GetItemMock(false, 1, 1);
        var secondItemMock = MockItemData.GetItemMock(false, 1, 1);
        
        var expectedSearchByTagActionResult = new InventoryActionResult(ItemsNotFound);

        IInventory inventory = new Inventory(InventoryName);
        inventory.TryAddItem(firstItemMock.Object);
        inventory.TryAddItem(secondItemMock.Object);

        var searchByTagActionResult = inventory.TryGetItemsByTag("ITEM_TAG_3");
        
        Assert.Equivalent(expectedSearchByTagActionResult, searchByTagActionResult);
    }
}