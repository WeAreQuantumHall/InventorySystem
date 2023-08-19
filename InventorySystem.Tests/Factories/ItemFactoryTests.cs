using System;
using InventorySystem.Abstractions;
using InventorySystem.Factories;
using Xunit;

namespace InventorySystem.Tests.Factories;

public class ItemFactoryTests
{
    [Fact]
    public void CreateItem__ReturnsIItemCorrectlyPopulated()
    {
        const string name = "TEST_ITEM_NAME";
        const string identifier = "TEST_ITEM_IDENTIFIER";
        const bool stackable = false;
        const int currentAmount = 0;
        const int maxAmount = 0;

        var returnedItem = ItemFactory.CreateItem(identifier, name);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(name, returnedItem.Name),
            () => Assert.Equal(identifier, returnedItem.Identifier),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack));
    }
    
    [Fact]
    public void CreateStackableItem__ReturnsIItemCorrectlyPopulated()
    {
        const string name = "TEST_ITEM_NAME";
        const string identifier = "TEST_ITEM_IDENTIFIER";
        const bool stackable = true;
        const int currentAmount = 0;
        const int maxAmount = 99;

        var returnedItem = ItemFactory.CreateStackableItem(identifier, name, stackable, currentAmount, maxAmount);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(name, returnedItem.Name),
            () => Assert.Equal(identifier, returnedItem.Identifier),
            () => Assert.Equal(stackable, returnedItem.Stackable),
            () => Assert.Equal(currentAmount, returnedItem.Stack),
            () => Assert.Equal(maxAmount, returnedItem.MaxStack));
    }
}