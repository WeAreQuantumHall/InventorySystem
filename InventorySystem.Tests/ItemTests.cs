using System;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using Xunit;

namespace InventorySystem.Tests;

public class ItemTests
{
    
    private const string Name = "TEST_ITEM_NAME";
    private const string Identifier = "TEST_ITEM_IDENTIFIER";
    private const ItemCategory Category = ItemCategory.Equipment;
    private static readonly string[] TagList = {"TEST_TAG_1", "TEST_TAG_2"};

    [Fact]
    public void New__ReturnsCorrectlyPopulatedItem()
    {
        const bool stackable = false;
        const int stack = 1;
        const int maxStack = 1;
        

        IItem item = new Item(Identifier, Name, Category, TagList)
        {
            Stackable = stackable,
            Stack = stack,
            MaxStack = maxStack,
        };

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, item.Id),
            () => Assert.Equal(Identifier, item.Identifier),
            () => Assert.Equal(Name, item.Name),
            () => Assert.Equal(stackable, item.Stackable),
            () => Assert.Equal(stack, item.Stack),
            () => Assert.Equal(maxStack, item.MaxStack),
            () => Assert.Equal(Category, item.ItemCategory),
            () => Assert.Equal(TagList, item.Tags));
    }

    [Fact]
    public void CanBeStackedOn_when_CurrentAmountEqualsMaxAmount__ReturnsFalse()
    {
        const bool stackable = true;
        const int currentAmount = 10;
        const int maxAmount = 10;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = stackable,
            Stack = currentAmount,
            MaxStack = maxAmount
        };

        Assert.False(item.CanBeStackedOn);
    }
    
    [Fact]
    public void CanBeStackedOn_when_NotStackable__ReturnsFalse()
    {
        const bool stackable = false;
        const int currentAmount = 1;
        const int maxAmount = 10;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = stackable,
            Stack = currentAmount,
            MaxStack = maxAmount
        };

        Assert.False(item.CanBeStackedOn);
    }
    
    [Fact]
    public void CanBeStackedOn_when_CurrentAmountIsLessThanMaxAmount_and_Stackable__ReturnsTrue()
    {
        const bool stackable = true;
        const int currentAmount = 1;
        const int maxAmount = 10;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = stackable,
            Stack = currentAmount,
            MaxStack = maxAmount
        };

        Assert.True(item.CanBeStackedOn);
    }
    
    [Fact]
    public void SplitStack_when_NotStackable__ReturnsOriginalItemNotSplit()
    {
        const int stackSize = 2;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = false,
            Stack = stackSize,
            MaxStack = 20
        };
        
        var splitItem = item.SplitStack(1);

        Assert.Multiple(
            () => Assert.Equal(item, splitItem),
            () => Assert.Equal(stackSize, item.Stack));
    }
    
    [Fact]
    public void SplitStack_when_Stackable_and_StackSize1__ReturnsItemToSplitNotSplit()
    {
        const int stackSize = 1;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = false,
            Stack = stackSize,
            MaxStack = 20
        };
        
        var splitItem = item.SplitStack(1);

        Assert.Multiple(
            () => Assert.Equal(item, splitItem),
            () => Assert.Equal(stackSize, item.Stack));
    }
    
    [Fact]
    public void SplitStack_when_SplitAmountGreaterThanStack__ReturnsItemToSplitNotSplit()
    {
        const int stackSize = 1;

        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = false,
            Stack = stackSize,
            MaxStack = 20
        };
        
        var splitItem = item.SplitStack(2);

        Assert.Multiple(
            () => Assert.Equal(item, splitItem),
            () => Assert.Equal(stackSize, item.Stack));
    }

    [Fact]
    public void SplitStack_whenCanBeSplit__ReturnsSplitItem_with_StackEqualSplitAmount_and_ItemToSplitWithExpectedStack()
    {
        const int expectedItemToSplitStack = 10;
        const int expectedSplitItemStack = 15;
        
        IItem itemToSplit = new Item(Identifier, Name, Category)
        {
            Stackable = true,
            Stack = 25,
            MaxStack = 30
        };

        var splitItem = itemToSplit.SplitStack(15);
        
        Assert.Multiple(
            () => Assert.NotEqual(itemToSplit, splitItem),
            () => Assert.Equal(itemToSplit.Name, splitItem.Name),
            () => Assert.Equal(itemToSplit.Identifier, splitItem.Identifier),
            () => Assert.Equal(itemToSplit.Stackable, splitItem.Stackable),
            () => Assert.Equal(itemToSplit.MaxStack, splitItem.MaxStack),
            () => Assert.Equal(expectedItemToSplitStack, itemToSplit.Stack),
            () => Assert.Equal(expectedSplitItemStack, splitItem.Stack));
    }

    [Fact]
    public void AddToStack_when_AbleToAddWholeAmount__Returns0_and_SetsExpectedStack()
    {
        const int expectedRemainingAmount = 0;
        const int expectedStack = 6;
        
        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = true,
            Stack = 1,
            MaxStack = 10
        };

        var remainingAmount = item.AddToStack(5);
        
        Assert.Multiple(
            () => Assert.Equal(expectedRemainingAmount, remainingAmount),
            () => Assert.Equal(expectedStack, item.Stack));
    }
    
    [Fact]
    public void AddToStack_when_NotAbleToAddWholeAmount__ReturnsExpectedRemainingAmount_and_SetsExpectedStack()
    {
        const int expectedRemainingAmount = 8;
        const int expectedStack = 10;
        
        IItem item = new Item(Identifier, Name, Category)
        {
            Stackable = true,
            Stack = 8,
            MaxStack = 10
        };

        var remainingAmount = item.AddToStack(10);
        
        Assert.Multiple(
            () => Assert.Equal(expectedRemainingAmount, remainingAmount),
            () => Assert.Equal(expectedStack, item.Stack));
    }

    [Fact]
    public void AddTag_when_CanAddTag_ReturnsTrue_and_AddsTag()
    {
        var expectedTagList = new[] {"TEST_TAG_1", "TEST_TAG_2", "TEST_TAG_3"};
        IItem item = new Item(Identifier, Name, Category, TagList);

        var hasBeenAdded = item.AddTag("TEST_TAG_3");

        Assert.Multiple(
            () => Assert.True(hasBeenAdded),
            () => Assert.Equal(expectedTagList, item.Tags));
    }
    
    [Fact]
    public void AddTag_when_CannotAddTag__ReturnsFalse_and_DoesNotAddTag()
    {
        var expectedTagList = new[] {"TEST_TAG_1", "TEST_TAG_2"};
        IItem item = new Item(Identifier, Name, Category, TagList);

        var hasBeenAdded = item.AddTag("TEST_TAG_2");

        Assert.Multiple(
            () => Assert.False(hasBeenAdded),
            () => Assert.Equal(expectedTagList, item.Tags));
    }
    
    [Fact]
    public void RemoveTag_when_CanRemoveTag_ReturnsTrue_and_RemovesTag()
    {
        var expectedTagList = new[] {"TEST_TAG_1"};
        IItem item = new Item(Identifier, Name, Category, TagList);

        var hasBeenRemoved = item.RemoveTag("TEST_TAG_2");

        Assert.Multiple(
            () => Assert.True(hasBeenRemoved),
            () => Assert.Equal(expectedTagList, item.Tags));
    }
    
    [Fact]
    public void RemoveTag_when_CannotRemoveTag__ReturnsFalse()
    {
        var expectedTagList = new[] {"TEST_TAG_1", "TEST_TAG_2"};
        IItem item = new Item(Identifier, Name, Category, TagList);

        var hasBeenRemoved = item.RemoveTag("TEST_TAG_3");

        Assert.Multiple(
            () => Assert.False(hasBeenRemoved),
            () => Assert.Equal(expectedTagList, item.Tags));
    }
}