using System;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using Moq;

namespace InventorySystem.Tests.Data;

public static class MockItemData
{
    private const string ItemName = "TEST_ITEM_NAME";
    private const string ItemIdentifier = "TEST_ITEM_IDENTIFIER";
    
    public static Mock<IItem> GetItemMock(bool stackable, int stack, int maxStack)
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
        itemMock.Setup(item => item.ItemCategory)
            .Returns(ItemCategory.Equipment);

        return itemMock;
    }
}