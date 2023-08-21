using System;
using InventorySystem.Abstractions;
using InventorySystem.ActionResults;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests;

public class InventoryTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";
    private const string ItemName = "TEST_ITEM_NAME";
    private const string ItemIdentifier = "TEST_ITEM_IDENTIFIER";

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
    public void TryGetItem_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        IInventory inventory = new Inventory(InventoryName);
        var expectedGetActionResult = new InventoryActionResult(ItemNotFound);

        var getActionResult = inventory.TryGetItem(id);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryGetItem_when_ItemIsPresent__ReturnsExpectedActionResult_with_Item()
    {
        var itemMock = GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemMock.Object);

        inventory.TryAddItem(itemMock.Object);
        var getActionResult = inventory.TryGetItem(itemMock.Object.Id);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryAddItem_when_NotStackable_And_InventoryAtMaxCapacity__ReturnsExpectedActionResult_an_NotAddItem()
    {
        var itemMock = GetItemMock(false, 1, 1);
        var itemToAddMock = GetItemMock(false, 1, 1);
        var expectedAddActionResult = new InventoryActionResult(InventoryAtCapacity, itemToAddMock.Object);
        
        IInventory inventory = new Inventory(InventoryName, 1);
        inventory.TryAddItem(itemMock.Object);
        var addActionResult = inventory.TryAddItem(itemToAddMock.Object);

        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }
    
    [Fact]
    public void TryAddItem_when_Stackable_And_InventoryAtMaxCapacity__ReturnsExpectedActionResult_an_NotAddItem()
    {
        var itemAtCloseToCapacityMock = GetItemMock(true, 8, 10);
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

        var itemToStackMock = GetItemMock(true, 10, 30);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(8)
            .Returns(8);
        
        var expectedAddActionResult = new InventoryActionResult(InventoryAtCapacity, itemToStackMock.Object);
        
        IInventory inventory = new Inventory(InventoryName, 1);
        inventory.TryAddItem(itemAtCloseToCapacityMock.Object);
        var addActionResult = inventory.TryAddItem(itemToStackMock.Object);

        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }

    [Fact]
    public void TryAddItem_when_NotStackable_and_NotPresent__ReturnsExpectedActionResult_and_AddsItem()
    {
        const int expectedCount = 1;
        var itemMock = GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemMock.Object);

        var addActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedCount, inventory.Count));
    }
    
    [Fact]
    public void TryAddItem_when_NotStackable_and_Present__ReturnsExpectedActionResult_and_NotAddItem()
    {
        const int expectedCount = 1;
        var itemMock = GetItemMock(false, 1, 1);
        IInventory inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAlreadyExists, itemMock.Object);
        
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
        var itemMock = GetItemMock(true, 10, 30);
        IInventory inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemMock.Object);

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
        var itemToBeStackedOnMock = GetItemMock(true, 10, 30);
        var itemToStackMock = GetItemMock(true, 10, 30);
        itemToBeStackedOnMock
            .Setup(item => item.Stack)
            .Returns(20);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(0);
        var expectedAddActionResult = new InventoryActionResult(ItemStacked, itemToBeStackedOnMock.Object);

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

        var maxCapacityItemMock = GetItemMock(true, 10, 10);
        var itemToBeStackedMock = GetItemMock(true, 10, 30);
        
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToBeStackedMock.Object);

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

        var itemAtCloseToCapacityMock = GetItemMock(true, 8, 10);
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

        var itemToStackMock = GetItemMock(true, 10, 30);
        itemToStackMock
            .SetupSequence(item => item.Stack)
            .Returns(10)
            .Returns(8)
            .Returns(8);
        
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToStackMock.Object);

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
    public void TrySplitItemStack_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        IInventory inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemNotFound);

        var splitActionResult = inventory.TrySplitItemStack(id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItemStack_when_ItemIsPresent_and_NotStackable__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToSplitMock = GetItemMock(false, 1, 1);
        itemToSplitMock
            .Setup(item => item.SplitStack(1))
            .Returns(itemToSplitMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackNotSplit, itemToSplitMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);
        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void
        TrySplitItemStack_when_ItemIsPresent_and_StackCannotBeSplit__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToSplitMock = GetItemMock(true, 1, 2);
        itemToSplitMock
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(itemToSplitMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackNotSplit, itemToSplitMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);

        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItem_when_ItemIsPresent_andStackCanBeSplit__ReturnsExpectedActionResult_and_AddsNewItem()
    {
        var splitItemMock = GetItemMock(true, 2, 2);
        var itemToSplitMock = GetItemMock(true, 1, 2);
        itemToSplitMock
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(splitItemMock.Object);

        IInventory inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackSplit, splitItemMock.Object);

        inventory.TryAddItem(itemToSplitMock.Object);

        var splitActionResult = inventory.TrySplitItemStack(itemToSplitMock.Object.Id, 1);

        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    private static Mock<IItem> GetItemMock(bool stackable, int stack, int maxStack)
    {
        var itemMock = new Mock<IItem>();
        itemMock.Setup(item => item.Id)
            .Returns(Guid.NewGuid());
        itemMock.Setup(item => item.Identifier)
            .Returns(ItemIdentifier);
        itemMock.Setup(item => item.Name)
            .Returns(ItemName);
        itemMock.Setup(item => item.Stackable)
            .Returns(stackable);
        itemMock.Setup(item => item.Stack)
            .Returns(stack);
        itemMock.Setup(item => item.MaxStack)
            .Returns(maxStack);
        itemMock.Setup(item => item.CanBeStackedOn)
            .Returns(stackable && stack < maxStack);
        return itemMock;
    }
}