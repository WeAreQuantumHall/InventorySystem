using System.Collections.Generic;
using InventorySystem.Tags;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class TagUtilsTests
{
    private static readonly int EquipmentTagCount = typeof(EquipmentTag).GetFields().Length;

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] {TagUtils.EquipmentTags.Tags, EquipmentTagCount}
        };
        
    
    
    [Theory]
    [MemberData(nameof(Data))]
    public void TagUtils_EnsureCorrectAmountOfTags(List<string> tags, int expectedCount)
    {
        Assert.Equal(tags.Count, expectedCount);
    } 
}