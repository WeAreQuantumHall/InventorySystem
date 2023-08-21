using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Inventories;
using Xunit;
using static InventorySystem.Abstractions.Enums.EquipmentCategory;

namespace InventorySystem.Tests.Inventories;

public class EquipmentInventoryTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";

    [Fact]
    public void New_WhenCategoryListNotProvided__ShouldCreateInventoryWithDefaultSlots()
    {
        const int expectedNumberOfSlots = 8;
        IInventory inventory = new EquipmentInventory(InventoryName);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    } 
    
    [Fact]
    public void New_WhenCategoryListProvided__ShouldCreateInventoryWithThatAmountOfSlots()
    {
        const int expectedNumberOfSlots = 4;

        var equipmentSlots = new List<EquipmentCategory> {Belt, Head, Chest, MainHand};
        IInventory inventory = new EquipmentInventory(InventoryName, equipmentSlots);
        
        Assert.Equal(expectedNumberOfSlots, inventory.Count);
    } 
}