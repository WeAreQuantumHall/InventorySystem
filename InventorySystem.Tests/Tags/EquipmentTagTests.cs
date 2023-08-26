using System.Collections.Generic;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class EquipmentTagTests
{
    [Fact]
    public void Instantiate__SetsExpectedEquipmentTags()
    {
        var expectedTags = new List<ITag> 
        {
            EquipmentTag.Head, 
            EquipmentTag.Chest, 
            EquipmentTag.Shoulders, 
            EquipmentTag.Hands,
            EquipmentTag.Belt,
            EquipmentTag.Legs,
            EquipmentTag.Feet,
            EquipmentTag.Offhand,
            EquipmentTag.MainHand,
            EquipmentTag.Neck,
            EquipmentTag.LeftEar,
            EquipmentTag.RightEar
        };

        var tags = EquipmentTag.Tags;
        
        Assert.Equal(expectedTags, tags);
    }

    [Fact]
    public void IsMember_whenIsEquipmentTag__ReturnsTrue()
    {
        var isMember = EquipmentTag.IsMember(EquipmentTag.Head);
        
        Assert.True(isMember);
    }
    
    [Fact]
    public void IsMember_whenIsNotEquipmentTag__ReturnsFalse()
    {
        var tagMock = new Mock<ITag>();
        
        var isMember = EquipmentTag.IsMember(tagMock.Object);
        
        Assert.False(isMember);
    }

    [Fact]
    public void GetMembers_when_NoMatchingTagsFound__ReturnsEmptyEnumerable()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(new List<ITag>());
        
        var members = EquipmentTag.GetMembers(tagListMock.Object);
        
        Assert.Empty(members);
    }

    [Fact]
    public void GetMembers_when_ItemsAreFound__ReturnsExpectedEnumerable()
    {
        var tagList = new List<ITag> {EquipmentTag.Head, EquipmentTag.Belt, new Mock<ITag>().Object};
        var expectedTagList = new List<ITag> {EquipmentTag.Head, EquipmentTag.Belt }; 
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        tagListMock
            .Setup(tl => tl.ContainsTag(EquipmentTag.Belt))
            .Returns(true);
        tagListMock
            .Setup(tl => tl.ContainsTag(It.IsAny<ITag>()))
            .Returns(false);
        tagListMock
            .Setup(tl => tl.ContainsTag(EquipmentTag.Head))
            .Returns(true);
        

        var members = EquipmentTag.GetMembers(tagListMock.Object);
        
        Assert.Equivalent(expectedTagList, members);
    }
}