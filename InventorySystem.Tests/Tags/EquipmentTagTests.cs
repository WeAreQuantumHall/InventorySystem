using System.Collections.Generic;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class EquipmentTagTests
{
    [Constructor]
    public void Equipment_tag_always_has_correct_equipment_tags_set()
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

    [HappyPath]
    public void Is_a_member_when_the_provided_tag_is_an_equipment_tag()
    {
        var isMember = EquipmentTag.IsMember(EquipmentTag.Head);
        
        Assert.True(isMember);
    }
    
    [UnhappyPath]
    public void Is_a_not_a_member_when_the_provided_tag_is_not_an_equipment_tag()
    {
        var tagMock = new Mock<ITag>();
        
        var isMember = EquipmentTag.IsMember(tagMock.Object);
        
        Assert.False(isMember);
    }

    
    [HappyPath]
    public void Getting_members_when_the_tag_list_contains_matching_tags_provides_those_matching_tags()
    {
        var tagList = new List<ITag> {EquipmentTag.Head, new Mock<ITag>().Object};
        var expectedTagList = new List<ITag> {EquipmentTag.Head}; 
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(tagList);
        tagListMock
            .Setup(tl => tl.ContainsTag(EquipmentTag.Head))
            .Returns(true);

        var members = EquipmentTag.GetMembers(tagListMock.Object);
        
        Assert.Equivalent(expectedTagList, members);
    }
    
    [UnhappyPath]
    public void Getting_members_when_the_tag_list_does_not_contain_any_matching_tags_provides_an_empty_list()
    {
        var tagListMock = new Mock<ITagList>();
        tagListMock
            .Setup(tl => tl.Tags)
            .Returns(new List<ITag>());
        
        var members = EquipmentTag.GetMembers(tagListMock.Object);
        
        Assert.Empty(members);
    }

}