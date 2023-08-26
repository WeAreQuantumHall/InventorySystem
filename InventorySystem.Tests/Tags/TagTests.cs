using System;

using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class TagTests
{
    [Fact]
    public void New__ReturnsExpectedTag()
    {
        const string expectedTagName = "tag-name";
        var expectedIdentifier = Guid.NewGuid();

        ITag tag = new Tag(expectedTagName, expectedIdentifier);

        Assert.Multiple(
            () => Assert.Equal(expectedTagName, tag.Name),
            () => Assert.Equal(expectedIdentifier, tag.Identifier));
    }

    [Fact]
    public void ToString__ReturnsTagName()
    {
        const string expectedTagName = "tag-name";

        ITag tag = new Tag(expectedTagName,  Guid.NewGuid());

        Assert.Equal(expectedTagName, tag.ToString());
    }
}