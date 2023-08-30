using InventorySystem.Items;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Items;

public class ItemStackTests
{
    [Constructor]
    public void Creating_a_new_item_stack_will_provide_an_item_stack_with_the_correct_values_set()
    {
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        
        Assert.Multiple(
            () => Assert.Equal(current, itemStack.Current),
            () => Assert.Equal(max,itemStack.Max));
    }

    [HappyPath]
    public void Can_be_stacked_on_when_stack_is_not_at_max()
    {
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        
        Assert.True(itemStack.CanBeStackedOn);
    }
    
    [HappyPath]
    public void Cannot_be_stacked_on_when_stack_is_at_max()
    {
        const int current = 20;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        
        Assert.False(itemStack.CanBeStackedOn);
    }
    
    [HappyPath]
    public void Adding_to_stack_when_full_amount_can_be_added_will_set_stack()
    {
        const int expectedCurrent = 15;
        const int expectedRemainder = 0;
        const int amountToAdd = 5;
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        var remainder = itemStack.AddToStack(amountToAdd);
        
        Assert.Multiple(
            () => Assert.Equal(expectedCurrent, itemStack.Current),
            () => Assert.Equal(expectedRemainder, remainder));
    }
    
    
    [UnhappyPath]
    public void Adding_to_stack_when_only_a_partial_amount_can_be_added_will_set_current_to_max_and_provide_remainder()
    {
        const int expectedRemainder = 5;
        const int amountToAdd = 15;
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        var remainder = itemStack.AddToStack(amountToAdd);
        
        Assert.Multiple(
            () => Assert.Equal(max, itemStack.Current),
            () => Assert.Equal(expectedRemainder, remainder));
    }

    [HappyPath]
    public void Setting_stack_when_amount_to_set_is_not_more_than_the_max_value_will_set_the_stack_to_that_value()
    {
        const int expectedRemainder = 0;
        const int amountToSet = 15;
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        var remainder = itemStack.SetStack(amountToSet);
        
        Assert.Multiple(
            () => Assert.Equal(amountToSet, itemStack.Current),
            () => Assert.Equal(expectedRemainder, remainder)); 
    }

    [UnhappyPath]
    public void Setting_the_stack_when_amount_to_set_is_more_than_max_will_set_stack_to_max_and_provide_a_remainder()
    {
        const int expectedRemainder = 5;
        const int amountToSet = 25;
        const int current = 10;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        var remainder = itemStack.SetStack(amountToSet);
        
        Assert.Multiple(
            () => Assert.Equal(max, itemStack.Current),
            () => Assert.Equal(expectedRemainder, remainder)); 
    }

    [HappyPath]
    public void Can_split_stack_when_split_amount_is_less_than_max_and_current_not_one()
    {
        const int current = 15;
        const int max = 20;
        const int splitAmount = 5;
        const int expectedCurrent = 10;

        var itemStack = new ItemStack(current, max);
        var hasSplitStack = itemStack.TrySplitStack(splitAmount);

        Assert.Multiple(
            () => Assert.True(hasSplitStack),
            () => Assert.Equal(expectedCurrent, itemStack.Current));
    }

    [UnhappyPath]
    public void Cannot_split_stack_when_current_stack_is_one()
    {
        const int current = 1;
        const int max = 20;

        var itemStack = new ItemStack(current, max);
        var hasSplitStack = itemStack.TrySplitStack(It.IsAny<int>());

        Assert.Multiple(
            () => Assert.False(hasSplitStack),
            () => Assert.Equal(current, itemStack.Current));
    }
    
    [UnhappyPath]
    public void Cannot_split_stack_when_current_stack_the_split_amount_is_more_than_max()
    {
        const int current = 10;
        const int max = 20;
        const int splitAmount = 30;

        var itemStack = new ItemStack(current, max);
        var hasSplitStack = itemStack.TrySplitStack(splitAmount);

        Assert.Multiple(
            () => Assert.False(hasSplitStack),
            () => Assert.Equal(current, itemStack.Current));
    }
}