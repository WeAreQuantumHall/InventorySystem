using System;
using System.Collections.Generic;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Factories;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Factories;

public class ItemFactoryTests
{
    private const string Name = "test-item-name";
    
    [HappyPath]
    public void Creating_a_new_item_provide_item_with_correctly_set_values()
    {
        var tagList = new List<ITag>
        {
            new Mock<ITag>().Object, new Mock<ITag>().Object
        };
        
        const bool stackable = false;
        const int currentAmount = 0;
        const int maxAmount = 0;

        var returnedItem = ItemFactory.CreateItem(Name, tagList);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(Name, returnedItem.Name),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack),
            () => Assert.Equal(tagList, returnedItem.TagList.Tags));
    }
    
    [HappyPath, Stackable]
    public void Creating_a_new_stackable_item_provide_item_with_correctly_set_values()
    {
        var tagList = new List<ITag>
        {
            new Mock<ITag>().Object, new Mock<ITag>().Object
        };
        
        const bool stackable = true;
        const int currentAmount = 1;
        const int maxAmount = 99;

        var returnedItem = ItemFactory.CreateStackableItem(
            Name, 
            stackable, 
            currentAmount, 
            maxAmount,
            tagList);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(Name, returnedItem.Name),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack),
            () => Assert.Equal(tagList, returnedItem.TagList.Tags));
    }
}