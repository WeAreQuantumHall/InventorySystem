using System;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Factories;
using Xunit;

namespace InventorySystem.Tests.Factories;

public class ItemFactoryTests
{
    private const string Name = "TEST_ITEM_NAME";
    private const string Identifier = "TEST_ITEM_IDENTIFIER";
    private const ItemCategory Category = ItemCategory.Equipment;
    private static readonly string[] TagList = {"TEST_TAG_1", "TEST_TAG_2"};
    
    [Fact]
    public void CreateItem__ReturnsIItemCorrectlyPopulated()
    {

        const bool stackable = false;
        const int currentAmount = 0;
        const int maxAmount = 0;

        var returnedItem = ItemFactory.CreateItem(Identifier, Name, Category, TagList);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(Name, returnedItem.Name),
            () => Assert.Equal(Identifier, returnedItem.Identifier),
            () => Assert.Equal(Category, returnedItem.ItemCategory),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack),
            () => Assert.Equal(TagList, returnedItem.TagList.Tags));
    }
    
    [Fact]
    public void CreateStackableItem__ReturnsIItemCorrectlyPopulated()
    {
        const bool stackable = true;
        const int currentAmount = 1;
        const int maxAmount = 99;

        var returnedItem = ItemFactory.CreateStackableItem(Identifier, Name, stackable, currentAmount, maxAmount,
            Category, TagList);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(Name, returnedItem.Name),
            () => Assert.Equal(Identifier, returnedItem.Identifier),
            () => Assert.Equal(Category, returnedItem.ItemCategory),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack),
            () => Assert.Equal(TagList, returnedItem.TagList.Tags));
        
    }
}