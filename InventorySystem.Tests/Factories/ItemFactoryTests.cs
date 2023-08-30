using System;
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
    public void Creating_a_new_item_will_provide_item_with_correctly_set_values()
    {
        var tagList = new Mock<ITagList>();
        tagList
            .Setup(tl => tl.Tags)
            .Returns(new[] {new Mock<ITag>().Object});
        var itemStackMock = new Mock<IItemStack>();
        itemStackMock
            .Setup(itemStack => itemStack.Current)
            .Returns(10);
        itemStackMock
            .Setup(itemStack => itemStack.Max)
            .Returns(10);
        
        var returnedItem = ItemFactory.CreateItem(
            Name, 
            tagList.Object.Tags,
            itemStackMock.Object);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.NotEqual(Guid.Empty, returnedItem.Id),
            () => Assert.Equal(Name, returnedItem.Name),
            () => Assert.Equivalent(itemStackMock.Object.Current, returnedItem.ItemStack!.Current),
            () => Assert.Equivalent(itemStackMock.Object.Max, returnedItem.ItemStack!.Max),
            () => Assert.Equal(tagList.Object.Tags, returnedItem.TagList.Tags));
    }
}