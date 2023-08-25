using System.Collections.Generic;
using System.Linq;
using InventorySystem.Tags;
using Xunit;

namespace InventorySystem.Tests.Tags;

public class EquipmentTagTests
{
    public static IEnumerable<object[]> TagData =>
        TagUtils.EquipmentTags.Tags.Select(t => new object[] {t});
        
 
    
    
    [Theory]
    [MemberData(nameof(TagData))]
    public void IsMember_whenTagFound__returnsTrue(string tag)
    {
        Assert.True(EquipmentTag.IsMember(tag));
    }


    public static IEnumerable<object[]> MappingData =>
        new[]
        {
            new object[] {EquipmentTag.Head, "Head"},
            new object[] {EquipmentTag.Chest, "Chest"},
            new object[] {EquipmentTag.Shoulders, "Shoulders"},
            new object[] {EquipmentTag.Hands, "Hands"},
            new object[] {EquipmentTag.Belt, "Belt"},
            new object[] {EquipmentTag.Legs, "Legs"},
            new object[] {EquipmentTag.Feet, "Feet"},
            new object[] {EquipmentTag.MainHand, "MainHand"},
            new object[] {EquipmentTag.OffHand, "OffHand"},
            new object[] {EquipmentTag.Neck, "Neck"},
            new object[] {EquipmentTag.LeftEar, "LeftEar"},
            new object[] {EquipmentTag.RightEar, "RightEar"}
        };
    
    [Theory]
    [MemberData(nameof(MappingData))]
    public void Instantiate__SetsExpectedEquipmentTags(string value, string expected)
    {
        Assert.Equal(expected, value);
    }
}