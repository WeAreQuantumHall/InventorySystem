using System;
using InventorySystem.Abstractions.Items;
using Moq;

namespace InventorySystem.Tests.Data;

public static class MockItemData
{
    private const string ItemName = "test-item-name";

    public static Mock<IItem> GetItemMock(bool stackable, int stack = 0, int maxStack = 0)
    {
        var itemMock = new Mock<IItem>();
        itemMock.Setup(item => item.Id)
            .Returns(Guid.NewGuid());
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