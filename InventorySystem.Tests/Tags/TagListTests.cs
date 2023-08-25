using System.Collections.Generic;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class TagListTests
{
    [Fact]
    public void New__CreatedNewTagListWithEmptyList()
    {
        ITagList tagList = new TagList();
        
        Assert.Empty(tagList.Tags);
    }
    
    [Fact]
    public void New_when_TagList__CreatesNewTagListWithProvidedTags()
    {
        var expectedTags = new[] {new Mock<ITag>().Object, new Mock<ITag>().Object};

        ITagList tagList = new TagList(expectedTags);

        Assert.Equal(expectedTags, tagList.Tags);
    }
    
    [Fact]
    public void IsMember_when_TagIsFound__ReturnsTrue()
    {
        var tagToFind = new Mock<ITag>();
        var expectedTags = new[] {tagToFind.Object, new Mock<ITag>().Object};
        
        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag(tagToFind.Object);

        Assert.True(isMember);
    }
    
    [Fact]
    public void IsMember_when_TagIsNotFound__ReturnsFalse()
    {
        var expectedTags = new[] {new Mock<ITag>().Object, new Mock<ITag>().Object};

        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag(new Mock<ITag>().Object);

        Assert.False(isMember);
    }

    [Fact]
    public void AddTag_when_TagDoesNotExist_ReturnsTrueAndAddsTag()
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

    [Fact]
    public void AddTag_when_TagAlreadyExists_ReturnsFalseAndDoesNotAddTag()
    {
        var tagToAddMock = new Mock<ITag>();
        var existingTags = new [] { tagToAddMock.Object };
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenAdded = tagList.AddTag(tagToAddMock.Object);
        
        Assert.Multiple(
            () => Assert.False(hasBeenAdded),
            () => Assert.Equal(existingTags, tagList.Tags));
    }

    [Fact]
    public void RemoveTag_when_TagExists__ReturnsTrueAndRemovesTag()
    {
        var existingTagMock = new Mock<ITag>();
        var existingTags = new [] {existingTagMock.Object};
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenRemoved = tagList.RemoveTag(existingTagMock.Object);
        
        Assert.Multiple(
            () => Assert.True(hasBeenRemoved),
            () => Assert.Empty(tagList.Tags));
    }
    
    [Fact]
    public void RemoveTag_when_TagDoesNotExist__ReturnsFalse()
    {
        var existingTags = new List<ITag> {new Mock<ITag>().Object};
        
        ITagList tagList = new TagList(existingTags);
        var hasBeenRemoved = tagList.RemoveTag(new Mock<ITag>().Object);


        Assert.Multiple(
            () => Assert.False(hasBeenRemoved),
            () => Assert.Equal(existingTags, tagList.Tags));
    }
}