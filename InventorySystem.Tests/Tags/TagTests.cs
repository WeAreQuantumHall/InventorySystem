using System;

using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;
using InventorySystem.Tests.AttributeTags;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class TagTests
{
    [Constructor]
    public void Creating_a_new_tag_provides_a_new_tag_with_the_correct_values_set()
    {
        const string expectedTagName = "tag-name";
        var expectedIdentifier = Guid.NewGuid();

        ITag tag = new Tag(expectedTagName, expectedIdentifier);

        Assert.Multiple(
            () => Assert.Equal(expectedTagName, tag.Name),
            () => Assert.Equal(expectedIdentifier, tag.Identifier));
    }

    [Fact]
    public void The_string_value_of_the_tag_will_be_the_tag_name()
    {
        const string expectedTagName = "tag-name";

        ITag tag = new Tag(expectedTagName,  Guid.NewGuid());

        Assert.Equal(expectedTagName, tag.ToString());
    }
}