using System;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Items;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Items;

public class ItemTests
{
    private const string Name = "test-item-name";
    private readonly Mock<ITagList> _tagListMock = new Mock<ITagList>();
    private readonly Mock<IItemStack> _itemStackMock = new Mock<IItemStack>();

    [Constructor]
    public void Create_new_item_will_provide_item_with_correctly_set_values()
    {
        IItem item = new Item(Name, _tagListMock.Object, _itemStackMock.Object);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, item.Id),
            () => Assert.Equal(Name, item.Name),
            () => Assert.Equivalent(_tagListMock.Object, item.TagList),
            () => Assert.Equivalent(_itemStackMock.Object, item.ItemStack));
    }
    
    [Constructor]
    public void Create_new_item_without_a_tag_list_provided_will_provide_item_with_an_empty_tag_list()
    {
        IItem item = new Item(Name);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, item.Id),
            () => Assert.Equal(Name, item.Name),
            () => Assert.Empty(item.TagList.Tags));
    }

    [Constructor]
    public void Create_new_item_without_a_item_stack_provided_will_provide_an_item_without_a_item_stack()
    {
        IItem item = new Item(Name);

        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, item.Id),
            () => Assert.Equal(Name, item.Name),
            () => Assert.Null(item.ItemStack));   
    }

    [HappyPath]
    public void Copy_provides_a_copy_of_the_item_with_a_different_id()
    {
        _tagListMock
            .Setup(tagList => tagList.Tags)
            .Returns(new[] {new Mock<ITag>().Object});
        _itemStackMock
            .Setup(itemStack => itemStack.Current)
            .Returns(10);
        _itemStackMock
            .Setup(itemStack => itemStack.Max)
            .Returns(20);
        
        IItem item = new Item(Name, _tagListMock.Object, _itemStackMock.Object);
        var itemCopy = item.Copy();
        
        Assert.Multiple(
            () => Assert.NotEqual(item.Id, itemCopy.Id),
            () => Assert.Equal(item.Name, itemCopy.Name),
            () => Assert.Equivalent(item.ItemStack, _itemStackMock.Object),
            () => Assert.Equivalent(item.TagList, _tagListMock.Object));
    }
       
    // [UnhappyPath]
    // public void The_item_cannot_be_stacked_on_when_the_item_stack_is_at_its_capacity()
    // {
    //     const bool stackable = true;
    //     const int currentAmount = 10;
    //     const int maxAmount = 10;
    //
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = stackable,
    //         Stack = currentAmount,
    //         MaxStack = maxAmount
    //     };
    //
    //     Assert.False(item.CanBeStackedOn);
    // }
    //
    // [UnhappyPath]
    // public void The_item_cannot_be_stacked_on_when_the_item_is_not_stackable()
    // {
    //     const bool stackable = false;
    //     const int currentAmount = 1;
    //     const int maxAmount = 10;
    //
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = stackable,
    //         Stack = currentAmount,
    //         MaxStack = maxAmount
    //     };
    //
    //     Assert.False(item.CanBeStackedOn);
    // }
    //
    // [HappyPath]
    // public void Trying_to_split_the_item_stack_when_it_can_be_split_provides_a_new_split_item_and_both_items_stack_will_be_set_correctly()
    // {
    //     const int expectedItemToSplitStack = 10;
    //     const int expectedSplitItemStack = 15;
    //     
    //     IItem itemToSplit = new Item(Name)
    //     {
    //         Stackable = true,
    //         Stack = 25,
    //         MaxStack = 30
    //     };
    //
    //     var splitItem = itemToSplit.SplitStack(15);
    //     
    //     Assert.Multiple(
    //         () => Assert.NotEqual(itemToSplit, splitItem),
    //         () => Assert.Equal(itemToSplit.Name, splitItem.Name),
    //         () => Assert.Equal(itemToSplit.Stackable, splitItem.Stackable),
    //         () => Assert.Equal(itemToSplit.MaxStack, splitItem.MaxStack),
    //         () => Assert.Equal(expectedItemToSplitStack, itemToSplit.Stack),
    //         () => Assert.Equal(expectedSplitItemStack, splitItem.Stack));
    // }
    //
    // [UnhappyPath]
    // public void Trying_to_split_the_item_stack_when_the_item_is_not_stackable_provides_the_original_item()
    // {
    //     const int stackSize = 2;
    //
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = false,
    //         Stack = stackSize,
    //         MaxStack = 20
    //     };
    //     
    //     var splitItem = item.SplitStack(1);
    //
    //     Assert.Multiple(
    //         () => Assert.Equal(item, splitItem),
    //         () => Assert.Equal(stackSize, item.Stack));
    // }
    //
    // [UnhappyPath]
    // public void Trying_to_split_the_item_stack_when_the_item_stack_is_one_provides_the_original_item()
    // {
    //     const int stackSize = 1;
    //
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = false,
    //         Stack = stackSize,
    //         MaxStack = 20
    //     };
    //     
    //     var splitItem = item.SplitStack(1);
    //
    //     Assert.Multiple(
    //         () => Assert.Equal(item, splitItem),
    //         () => Assert.Equal(stackSize, item.Stack));
    // }
    //
    // [UnhappyPath]
    // public void Trying_to_split_the_item_stack_when_the_amount_to_split_is_greater_than_the_stack_size_provides_the_original_item()
    // {
    //     const int stackSize = 1;
    //
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = false,
    //         Stack = stackSize,
    //         MaxStack = 20
    //     };
    //     
    //     var splitItem = item.SplitStack(2);
    //
    //     Assert.Multiple(
    //         () => Assert.Equal(item, splitItem),
    //         () => Assert.Equal(stackSize, item.Stack));
    // }
    //
    // [HappyPath]
    // public void Adding_to_the_stack_when_the_whole_amount_can_be_added_will_provide_zero_remaining_stack_and_set_the_stack_on_the_item()
    // {
    //     const int expectedRemainingAmount = 0;
    //     const int expectedStack = 6;
    //     
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = true,
    //         Stack = 1,
    //         MaxStack = 10
    //     };
    //
    //     var remainingAmount = item.AddToStack(5);
    //     
    //     Assert.Multiple(
    //         () => Assert.Equal(expectedRemainingAmount, remainingAmount),
    //         () => Assert.Equal(expectedStack, item.Stack));
    // }
    //
    // [UnhappyPath]
    // public void Adding_to_the_stack_when_unable_to_add_full_amount_will_provide_remaining_stack_and_set_item_stack_to_capacity()
    // {
    //     const int expectedRemainingAmount = 8;
    //     const int expectedStack = 10;
    //     
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = true,
    //         Stack = 8,
    //         MaxStack = 10
    //     };
    //
    //     var remainingAmount = item.AddToStack(10);
    //     
    //     Assert.Multiple(
    //         () => Assert.Equal(expectedRemainingAmount, remainingAmount),
    //         () => Assert.Equal(expectedStack, item.Stack));
    // }
    //
    // [HappyPath]
    // public void Setting_the_stack_when_the_amount_is_less_than_capacity_will_provide_remaining_stack_of_zero_and_set_the_stack()
    // {
    //     const int expectedRemainingAmount = 0;
    //     const int expectedStack = 9;
    //     
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = true,
    //         Stack = 5,
    //         MaxStack = 10
    //     };
    //
    //     var remainingAmount = item.SetStack(9);
    //     
    //     Assert.Multiple(
    //         () => Assert.Equal(expectedRemainingAmount, remainingAmount),
    //         () => Assert.Equal(expectedStack, item.Stack));
    // }
    //
    // [HappyPath]
    // public void Setting_the_stack_when_the_amount_is_more_than_capacity_will_provide_remaining_stack_and_set_the_stack()
    // {
    //     const int expectedRemainingAmount = 5;
    //     const int expectedStack = 10;
    //     
    //     IItem item = new Item(Name)
    //     {
    //         Stackable = true,
    //         Stack = 5,
    //         MaxStack = 10
    //     };
    //
    //     var remainingAmount = item.SetStack(15);
    //     
    //     Assert.Multiple(
    //         () => Assert.Equal(expectedRemainingAmount, remainingAmount),
    //         () => Assert.Equal(expectedStack, item.Stack));
    // }
    //
    [HappyPath]
    public void Item_does_add_the_tag_to_the_tag_list_when_the_tag_can_be_added()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.AddTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenAdded = item.AddTag(new Mock<ITag>().Object);

        Assert.True(hasBeenAdded);
    }
    
    [UnhappyPath]
    public void Item_does_not_add_the_tag_to_the_tag_list_when_the_tag_cannot_be_added()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(false);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenAdded = item.AddTag(new Mock<ITag>().Object);

        Assert.False(hasBeenAdded);
    }
    
    [HappyPath]
    public void Item_does_remove_the_tag_from_the_tag_list_when_the_tag_can_be_removed()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.RemoveTag(new Mock<ITag>().Object);

        Assert.True(hasBeenRemoved);
    }
    
    [UnhappyPath]
    public void Item_does_not_remove_the_tag_from_the_tag_list_when_the_tag_cannot_be_removed()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.RemoveTag(It.IsAny<ITag>()))
            .Returns(false);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.RemoveTag(new Mock<ITag>().Object);

        Assert.False(hasBeenRemoved);
    }

    [HappyPath]
    public void Item_does_contain_the_tag_when_the_tag_list_contains_the_tag()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(item => item.ContainsTag(It.IsAny<ITag>()))
            .Returns(true);
        
        IItem item = new Item(Name, tagListMock.Object);
        var hasBeenRemoved = item.ContainsTag(new Mock<ITag>().Object);

        Assert.True(hasBeenRemoved);
    }
    
    [UnhappyPath]
    public void Item_does_not_contain_the_tag_when_the_tag_list_does_not_contain_the_tag()
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