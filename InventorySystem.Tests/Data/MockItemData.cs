using System;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using Moq;

namespace InventorySystem.Tests.Data;

public static class MockItemData
{
    private const string ItemName = "test-item-name";

    public static Mock<IItem> GetItemMock(
        Mock<ITagList>? tagList = null,
        Mock<IItemStack>? itemStack = null)
    {
        var itemMock = new Mock<IItem>();
        itemMock
            .Setup(item => item.Id)
            .Returns(Guid.NewGuid());
        itemMock
            .Setup(item => item.Name)
            .Returns(ItemName);

        if (itemStack != null)
        {
            itemMock
                .Setup(item => item.ItemStack)
                .Returns(itemStack.Object);
        }

        if (tagList == null)
        {
            tagList = new Mock<ITagList>();
            tagList
                .Setup(tl => tl.Tags)
                .Returns(Array.Empty<ITag>());
        }
        
        itemMock
            .Setup(item => item.TagList)
            .Returns(tagList.Object);
        
        return itemMock;
    }
}