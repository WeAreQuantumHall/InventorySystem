using System;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Items;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Items;

public class ItemTests
{
    
    private const string Name = "test-item-name";

    [Fact]
    public void Empty__ReturnsExpectedItem()
    {
        const string emptyItemString = "empty-item";

        var emptyItem = Item.Empty;

        Assert.Equal(emptyItemString, emptyItem.Name);
    }

    [Fact]
    public void New__ReturnsCorrectlyPopulatedItem()
    {
        const bool stackable = false;
        const int stack = 1;
        const int maxStack = 1;
        var tagListMock = new Mock<ITagList>();    
        
        IItem item = new Item(Name, tagListMock.Object)
        {
            Stackable = stackable,
            Stack = stack,
            MaxStack = maxStack
        };

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, item.Id),
            () => Assert.Equal(Name, item.Name),
            () => Assert.Equal(stackable, item.Stackable),
            () => Assert.Equal(stack, item.Stack),
            () => Assert.Equal(maxStack, item.MaxStack),
            () => Assert.Equivalent(tagListMock.Object, item.TagList));
    }

    [Fact]
    public void CanBeStackedOn_when_CurrentAmountEqualsMaxAmount__ReturnsFalse()
    {
        const bool stackable = true;
        const int currentAmount = 10;
        const int maxAmount = 10;

        IItem item = new Item(Name)
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

        IItem item = new Item(Name)
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

        IItem item = new Item(Name)
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

        IItem item = new Item(Name)
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

        IItem item = new Item(Name)
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

        IItem item = new Item(Name)
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
        
        IItem itemToSplit = new Item(Name)
        {
            Stackable = true,
            Stack = 25,
            MaxStack = 30
        };

        var splitItem = itemToSplit.SplitStack(15);
        
        Assert.Multiple(
            () => Assert.NotEqual(itemToSplit, splitItem),
            () => Assert.Equal(itemToSplit.Name, splitItem.Name),
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
        
        IItem item = new Item(Name)
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
        
        IItem item = new Item(Name)
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
    public void AddTag_when_CanAddTag__ReturnsTrue()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.AddTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenAdded = item.AddTag(new Mock<ITag>().Object);

        Assert.True(hasBeenAdded);
    }
    
    [Fact]
    public void AddTag_when_CannotAddTag__ReturnsFalse()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(false);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenAdded = item.AddTag(new Mock<ITag>().Object);

        Assert.False(hasBeenAdded);
    }
    
    [Fact]
    public void RemoveTag_when_CanRemoveTag__ReturnsTrue()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.RemoveTag(new Mock<ITag>().Object);

        Assert.True(hasBeenRemoved);
    }
    
    [Fact]
    public void RemoveTag_when_CannotRemoveTag__ReturnsFalse()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(false);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.RemoveTag(new Mock<ITag>().Object);

        Assert.False(hasBeenRemoved);
    }

    [Fact]
    public void ContainsTag_when_TagListContainsTag__ReturnsTrue()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.ContainsTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.ContainsTag(new Mock<ITag>().Object);

        Assert.True(hasBeenRemoved);
    }
    
    [Fact]
    public void ContainsTag_when_TagListDoesNotContainsTag__ReturnsFalse()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.ContainsTag(It.IsAny<ITag>()))
            .Returns(false);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.ContainsTag(new Mock<ITag>().Object);

        Assert.False(hasBeenRemoved);
    }
}