using InventorySystem.Abstractions;
using InventorySystem.Tags;
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
    public void New_with_StringEnumerableProvided__CreatesNewTagListWithProvidedTags()
    {
        var expectedTags = new [] {"TestTag", "TestTag2"};

        ITagList tagList = new TagList(expectedTags);

        Assert.Equal(expectedTags, tagList.Tags);
    }
    
    [Fact]
    public void IsMember_when_TagIsFound__ReturnsTrue()
    {
        var expectedTags = new [] {"TestTag", "TestTag2"};

        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag("TestTag");

        Assert.True(isMember);
    }
    
    [Fact]
    public void IsMember_when_TagIsNotFound__ReturnsFalse()
    {
        var expectedTags = new [] {"TestTag", "TestTag2"};

        ITagList tagList = new TagList(expectedTags);
        var isMember = tagList.ContainsTag("TestTag3");

        Assert.False(isMember);
    }

    [Fact]
    public void AddTag_when_TagDoesNotExist_ReturnsTrueAndAddsTag()
    {
        var existingTags = new [] {"TestTag", "TestTag2"};
        var expectedTags = new [] {"TestTag", "TestTag2", "TestTag3"};

        ITagList tagList = new TagList(existingTags);
        var hasBeenAdded = tagList.AddTag("TestTag3");
        
        Assert.Multiple(
            () => Assert.True(hasBeenAdded),
            () => Assert.Equal(expectedTags, tagList.Tags));
    }

    [Fact]
    public void AddTag_when_TagAlreadyExists_ReturnsFalseAndDoesNotAddTag()
    {
        var tags = new [] {"TestTag", "TestTag2"};

        ITagList tagList = new TagList(tags);
        var hasBeenAdded = tagList.AddTag("TestTag2");
        
        Assert.Multiple(
            () => Assert.False(hasBeenAdded),
            () => Assert.Equal(tags, tagList.Tags));
    }

    [Fact]
    public void RemoveTag_when_TagExists__ReturnsTrueAndRemovesTag()
    {
        var existingTags = new [] {"TestTag", "TestTag2"};
        var expectedTags = new [] {"TestTag"};

        ITagList tagList = new TagList(existingTags);
        var hasBeenRemoved = tagList.RemoveTag("TestTag2");
        
        Assert.Multiple(
            () => Assert.True(hasBeenRemoved),
            () => Assert.Equal(expectedTags, tagList.Tags));
    }
    
    [Fact]
    public void RemoveTag_when_TagDoesNotExist__ReturnsFalse()
    {
        var tags = new [] {"TestTag", "TestTag2"};
        
        ITagList tagList = new TagList(tags);
        var hasBeenRemoved = tagList.RemoveTag("TestTag3");
        
        Assert.Multiple(
            () => Assert.False(hasBeenRemoved),
            () => Assert.Equal(tags, tagList.Tags));
    }
}