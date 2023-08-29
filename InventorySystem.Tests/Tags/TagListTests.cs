using System.Collections.Generic;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class TagListTests
{
    [Constructor]
    public void Creating_a_new_tag_list_without_providing_tags_provides_an_empty_tag_list()
        
    {
        ITagList tagList = new TagList();
        
        Assert.Empty(tagList.Tags);
    }
    
    [Constructor]
    public void Creating_a_new_tag_list_when_providing_a_list_of_tags_provides_a_tag_list_containing_those_tags()

    {
        var expectedTags = new[] {new Mock<ITag>().Object, new Mock<ITag>().Object};

        ITagList tagList = new TagList(expectedTags);

        Assert.Equal(expectedTags, tagList.Tags);
    }
    
    [HappyPath]
    public void Is_a_member_when_the_provided_tag_is_found_in_the_tag_list()
    {
        var tagToFind = new Mock<ITag>();
        var expectedTags = new[] {tagToFind.Object, new Mock<ITag>().Object};
        
        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag(tagToFind.Object);

        Assert.True(isMember);
    }
    
    [UnhappyPath]
    public void Is_not_a_member_when_the_provided_tag_is_not_found_in_the_tag_list()
    {
        var expectedTags = new[] {new Mock<ITag>().Object, new Mock<ITag>().Object};

        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag(new Mock<ITag>().Object);

        Assert.False(isMember);
    }

    [HappyPath]
    public void Can_add_tag_when_the_provided_tag_is_not_already_present_in_the_tag_list_and_will_add_the_tag()
    {
        var existingTagMock = new Mock<ITag>();
        var tagToAddMock = new Mock<ITag>();
        var existingTags = new [] {existingTagMock.Object};
        var expectedTags = new [] {existingTagMock.Object, tagToAddMock.Object};

        ITagList tagList = new TagList(existingTags);
        var hasBeenAdded = tagList.AddTag(tagToAddMock.Object);
        
        Assert.Multiple(
            () => Assert.True(hasBeenAdded),
            () => Assert.Equal(expectedTags, tagList.Tags));
    }

    [UnhappyPath]
    public void Can_not_add_tag_when_the_provided_tag_is_already_present_in_the_tag_list_and_will_not_add_the_tag()
    {
        var tagToAddMock = new Mock<ITag>();
        var existingTags = new [] { tagToAddMock.Object };
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenAdded = tagList.AddTag(tagToAddMock.Object);
        
        Assert.Multiple(
            () => Assert.False(hasBeenAdded),
            () => Assert.Equal(existingTags, tagList.Tags));
    }

    [HappyPath]
    public void Can_remove_tag_when_the_provided_tag_is_already_present_in_the_tag_list_and_will_remove_add_the_tag()
    {
        var existingTagMock = new Mock<ITag>();
        var existingTags = new [] {existingTagMock.Object};
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenRemoved = tagList.RemoveTag(existingTagMock.Object);
        
        Assert.Multiple(
            () => Assert.True(hasBeenRemoved),
            () => Assert.Empty(tagList.Tags));
    }
    
    [HappyPath]
    public void Cannot_remove_tag_when_the_provided_tag_is_already_present_in_the_tag_list_and_will_remove_add_the_tag()
    {
        var existingTags = new List<ITag> {new Mock<ITag>().Object};
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenRemoved = tagList.RemoveTag(new Mock<ITag>().Object);


        Assert.Multiple(
            () => Assert.False(hasBeenRemoved),
            () => Assert.Equal(existingTags, tagList.Tags));
    }
}