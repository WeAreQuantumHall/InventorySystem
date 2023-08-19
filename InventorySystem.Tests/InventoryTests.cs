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
    public void New__CreatesCorrectlyPopulatedValues()
    {
        var inventory = new Inventory(InventoryName);
        
        Assert.NotEqual(Guid.Empty, inventory.Id);
        Assert.Equal(InventoryName, inventory.Name);
    }

    [Fact]
    public void SetName__CorrectlySetsName()
    {
        const string newInventoryName = "TEST_NAME_NEW";
        var inventory = new Inventory(InventoryName);
        
        inventory.SetName(newInventoryName);
        
        Assert.Equal(newInventoryName, inventory.Name);
    }
    
    [Fact]
    public void TryGetItem_when_ItemIsPresent__ReturnsExpectedActionResult_with_Item()
    {
        var itemToGetId = Guid.NewGuid();
        var itemToGet = new Mock<IItem>();
        itemToGet
            .SetupGet(item => item.Id)
            .Returns(itemToGetId);
        itemToGet
            .SetupGet(item => item.Stackable)
            .Returns(false);
        
        var inventory = new Inventory(InventoryName);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToGet.Object);

        inventory.TryAddItem(itemToGet.Object);

        var getActionResult = inventory.TryGetItem(itemToGetId);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }
    
    [Fact]
    public void TryGetItem_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        var inventory = new Inventory(InventoryName);
        var expectedGetActionResult = new InventoryActionResult(ItemNotFound);
        
        var getActionResult = inventory.TryGetItem(id);

        Assert.Equivalent(expectedGetActionResult, getActionResult);
    }

    [Fact]
    public void TryAddItem_when_NotStackable_and_NotPresent__ReturnsExpectedActionResult_and_AddItem()
    {
        var itemToAddId = Guid.NewGuid();
        var itemToAdd = new Mock<IItem>();
        itemToAdd
            .SetupGet(item => item.Id)
            .Returns(itemToAddId);
        itemToAdd
            .SetupGet(item => item.Stackable)
            .Returns(false);

        var inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToAdd.Object);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToAdd.Object);

        var addActionResult = inventory.TryAddItem(itemToAdd.Object);
        var getActionResult = inventory.TryGetItem(itemToAddId);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equivalent(expectedGetActionResult, getActionResult));
    }
    
    [Fact]
    public void TryAddItem_when_NotStackable_and_Present__ReturnsExpectedActionResult_and_NotAddItem()
    {
        var itemToAddId = Guid.NewGuid();
        var itemToAdd = new Mock<IItem>();
        itemToAdd
            .SetupGet(item => item.Id)
            .Returns(itemToAddId);
        itemToAdd
            .SetupGet(item => item.Stackable)
            .Returns(false);
        
        var inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAlreadyExists, itemToAdd.Object);
        
        inventory.TryAddItem(itemToAdd.Object);
        var addActionResult = inventory.TryAddItem(itemToAdd.Object);
        
        Assert.Equivalent(expectedAddActionResult, addActionResult);
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarItemNotPresent__ReturnsExpectedActionResult_and_AddItem()
    {
        var itemToAddId = Guid.NewGuid();
        var itemToAdd = new Mock<IItem>();
        itemToAdd
            .SetupGet(item => item.Id)
            .Returns(itemToAddId);
        itemToAdd
            .SetupGet(item => item.Identifier)
            .Returns(ItemIdentifier);
        itemToAdd
            .SetupGet(item => item.Stackable)
            .Returns(true);
        itemToAdd
            .SetupGet(item => item.Stack)
            .Returns(1);
        itemToAdd
            .SetupGet(item => item.Stack)
            .Returns(99);
        
        var inventory = new Inventory(InventoryName);
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToAdd.Object);
        var expectedGetActionResult = new InventoryActionResult(ItemRetrieved, itemToAdd.Object);
        
        var addActionResult = inventory.TryAddItem(itemToAdd.Object);
        var getActionResult = inventory.TryGetItem(itemToAddId);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equivalent(expectedGetActionResult, getActionResult));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_ShouldBeStacked__ReturnExpectedActionResult_and_NotAddItem()
    {
        const int expectedStack = 6;
        
        var stackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 1,
            MaxStack = 99
        };
        var itemToAdd = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 5,
            MaxStack = 99
        };
        
        var expectedAddActionResult = new InventoryActionResult(ItemStacked, stackableItem);

        var inventory = new Inventory(InventoryName);
        inventory.TryAddItem(stackableItem);
        
        var addActionResult = inventory.TryAddItem(itemToAdd);
        var getActionResult = inventory.TryGetItem(itemToAdd.Id);
        
        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(expectedStack, addActionResult.Item!.Stack),
            () => Assert.Equal(ItemNotFound, getActionResult.Result));
    }
    
    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarMaxStackItemPresent__ShouldReturnExpectedActionResult_and_AddItem()
    {
        var stackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 99,
            MaxStack = 99
        };
        var itemToAdd = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 5,
            MaxStack = 99
        };
        var expectedAddActionResult = new InventoryActionResult(ItemAdded, itemToAdd);

        var inventory = new Inventory(InventoryName);
        inventory.TryAddItem(stackableItem);
        
        var addActionResult = inventory.TryAddItem(itemToAdd);
        var getActionResult = inventory.TryGetItem(itemToAdd.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(ItemRetrieved, getActionResult.Result));
    }
    
    [Fact]
    public void TryAddItem_when_Stackable_and_SimilarItemPresent_and_ShouldHaveSpilled__ShouldReturnExpectedActionResult_and_AddItem()
    {
        const int expectedStack = 4; 
        var stackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 98,
            MaxStack = 99
        };
        var itemToAdd = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 5,
            MaxStack = 99
        };
        var expectedAddActionResult = new InventoryActionResult(ItemStackedAndSpilled, itemToAdd);
        
        var inventory = new Inventory(InventoryName);
        inventory.TryAddItem(stackableItem);
        
        var addActionResult = inventory.TryAddItem(itemToAdd);
        var getActionResult = inventory.TryGetItem(itemToAdd.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(ItemRetrieved, getActionResult.Result),
            () => Assert.Equal(expectedStack, getActionResult.Item!.Stack),
            () => Assert.Equal(stackableItem.MaxStack, stackableItem.Stack));
    }

    [Fact]
    public void TryAddItem_when_Stackable_and_ShouldFillStacks_and_Spill__ReturnsExpectedActionResult_andAddItem()
    {
        const int expectedStack = 3; 
        var firstSimilarStackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 98,
            MaxStack = 99
        };
        var secondSimilarStackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 98,
            MaxStack = 99
        };
        var itemToAdd = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 5,
            MaxStack = 99
        };
        var expectedAddActionResult = new InventoryActionResult(ItemStackedAndSpilled, itemToAdd);
        
        var inventory = new Inventory(InventoryName);
        inventory.TryAddItem(firstSimilarStackableItem);
        inventory.TryAddItem(secondSimilarStackableItem);
        
        var addActionResult = inventory.TryAddItem(itemToAdd);
        var getActionResult = inventory.TryGetItem(itemToAdd.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(ItemRetrieved, getActionResult.Result),
            () => Assert.Equal(expectedStack, getActionResult.Item!.Stack),
            () => Assert.Equal(firstSimilarStackableItem.MaxStack, firstSimilarStackableItem.Stack),
            () => Assert.Equal(secondSimilarStackableItem.MaxStack, secondSimilarStackableItem.Stack));
    }
    
    [Fact]
    public void TryAddItem_when_Stackable_and_ShouldFillStacks_and_NotSpill__ReturnsExpectedActionResult_andNotAddItem()
    {
        const int expectedStack = 6; 
        var firstSimilarStackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 98,
            MaxStack = 99
        };
        var secondSimilarStackableItem = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 2,
            MaxStack = 99
        };
        var itemToAdd = new Item(ItemIdentifier, ItemName)
        {
            Stackable = true,
            Stack = 5,
            MaxStack = 99
        };
        var expectedAddActionResult = new InventoryActionResult(ItemStacked, secondSimilarStackableItem);
        
        var inventory = new Inventory(InventoryName);
        inventory.TryAddItem(firstSimilarStackableItem);
        inventory.TryAddItem(secondSimilarStackableItem);
        
        var addActionResult = inventory.TryAddItem(itemToAdd);
        var getActionResult = inventory.TryGetItem(itemToAdd.Id);

        Assert.Multiple(
            () => Assert.Equivalent(expectedAddActionResult, addActionResult),
            () => Assert.Equal(ItemNotFound, getActionResult.Result),
            () => Assert.Equal(firstSimilarStackableItem.MaxStack, firstSimilarStackableItem.Stack),
            () => Assert.Equal(expectedStack, secondSimilarStackableItem.Stack));
    }

    

    [Fact]
    public void TrySplitItemStack_when_ItemIsNotPresent__ReturnsExpectedActionResult_with_NoItem()
    {
        var id = Guid.NewGuid();
        var inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemNotFound);

        var splitActionResult = inventory.TrySplitItemStack(id, 1);
        
        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItemStack_when_ItemIsPresent_and_NotStackable__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var itemToSplit = new Item(ItemName, ItemIdentifier);
        var inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackNotSplit, itemToSplit);

        inventory.TryAddItem(itemToSplit);
        var splitActionResult = inventory.TrySplitItemStack(itemToSplit.Id, 1);
        
        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }
    
    [Fact]
    public void TrySplitItemStack_when_ItemIsPresent_and_StackCannotBeSplit__ReturnsExpectedActionResult_with_OriginalItem()
    {
        var id = Guid.NewGuid();
        var itemToSplit = new Mock<IItem>();
        itemToSplit
            .SetupGet(item => item.Id)
            .Returns(id);
        itemToSplit
            .SetupGet(item => item.Stackable)
            .Returns(true);
        itemToSplit
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(itemToSplit.Object);
        
        var inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackNotSplit, itemToSplit.Object);
        
        inventory.TryAddItem(itemToSplit.Object);
        
        var splitActionResult = inventory.TrySplitItemStack(id, 1);
        
        Assert.Equivalent(expectedSplitActionResult, splitActionResult);
    }

    [Fact]
    public void TrySplitItem_when_ItemIsPresent_andStackCanBeSplit__ReturnsExpectedActionResult_and_AddsNewItem()
    {
        var itemToSplitId = Guid.NewGuid();
        var splitItemId = Guid.NewGuid();
        
        var splitItem = new Mock<IItem>();
        splitItem
            .SetupGet(item => item.Id)
            .Returns(splitItemId);
        
        var itemToSplit = new Mock<IItem>();
        itemToSplit
            .SetupGet(item => item.Id)
            .Returns(itemToSplitId);
        itemToSplit
            .SetupGet(item => item.Stackable)
            .Returns(true);
        itemToSplit
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(splitItem.Object);
        
        var inventory = new Inventory(InventoryName);
        var expectedSplitActionResult = new InventoryActionResult(ItemStackSplit, splitItem.Object);
        
        inventory.TryAddItem(itemToSplit.Object);
        
        var splitActionResult = inventory.TrySplitItemStack(itemToSplitId, 1);
        var getActionResult = inventory.TryGetItem(splitActionResult.Item!.Id);
        
        Assert.Multiple(
            () => Assert.Equivalent(expectedSplitActionResult, splitActionResult),
            () => Assert.Equal(ItemRetrieved, getActionResult.Result));
    }
}