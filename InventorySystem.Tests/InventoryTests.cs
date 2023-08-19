using System;
using InventorySystem.Abstractions;
using InventorySystem.Tests.Stubs;
using Xunit;

namespace InventorySystem.Tests;

public class InventoryTests
{
    private const string InventoryName = "TEST_INVENTORY_NAME";
    
    [Fact]
    public void New__CreatesCorrectlyPopulatedValues()
    {
        var inventory = new Inventory(InventoryName);
        
        Assert.NotEqual(Guid.Empty, inventory.Id);
        Assert.Equal(InventoryName, inventory.Name);
    }

    [Fact]
    public void SetName__CorrectlySetsName()
    {
        const string newInventoryName = "TEST_NAME_NEW";
        var inventory = new Inventory(InventoryName);
        
        inventory.SetName(newInventoryName);
        
        Assert.Equal(newInventoryName, inventory.Name);
    }

    [Fact]
    public void TryAddItem_when_ItemIsNotAlreadyPresent__ReturnsTrue_and_AddsItem()
    {
        var item = new ItemStub();
        
        var inventory = new Inventory(InventoryName);

        var hasItemBeenAdded = inventory.TryAddItem(item);
        var isItemFound = inventory.TryGetItem(item.Id, out _);        
        
        Assert.Multiple(
            () => Assert.True(hasItemBeenAdded),
            () => Assert.True(isItemFound));
    }
    
    [Fact]
    public void TryAddItem_when_ItemIsAlreadyPresent__ReturnsFalse_and_DoesNotAddItem()
    {
        var item = new ItemStub();
        var inventory = new Inventory(InventoryName);

        inventory.TryAddItem(item);
        var hasItemBeenAdded = inventory.TryAddItem(item);
        
        Assert.False(hasItemBeenAdded);
    }

    [Fact]
    public void TryGetItem_when_ItemIsPresent__ReturnsTrue_and_IReadOnlyItem()
    {
        var inventory = new Inventory(InventoryName);
        var item = new ItemStub();

        inventory.TryAddItem(item);

        var isItemFound = inventory.TryGetItem(item.Id, out var returnedItem);

        Assert.Multiple(
            () => Assert.IsAssignableFrom<IItem>(returnedItem),
            () => Assert.True(isItemFound),
            () => Assert.Equal(item, returnedItem));
    }
    
    [Fact]
    public void TryGetItem_when_ItemIsNotPresent__ReturnsFalse_andNullItem()
    {
        var id = Guid.NewGuid();
        
        var inventory = new Inventory(InventoryName);
        
        var isItemFound = inventory.TryGetItem(id, out var returnedItem);

        Assert.Multiple(
            () => Assert.False(isItemFound),
            () => Assert.Null(returnedItem));
    }
}