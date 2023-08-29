using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Items;
using InventorySystem.Services.Inventory;
using InventorySystem.Tests.AttributeTags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Services.Inventory;

public class StackableInventoryServiceTests
{
    private readonly Mock<IDictionary<Guid, IItem>> _items = new Mock<IDictionary<Guid, IItem>>();
    
    [HappyPath]
    public void Trying_to_add_a_non_stackable_item_which_has_not_already_been_added_will_add_the_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Once()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [HappyPath, Stackable]
    public void
        Trying_to_add_a_stackable_item_when_similar_item_not_in_inventory_and_inventory_is_not_full_will_add_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);

        _items.Setup(items => items.Values)
            .Returns(Array.Empty<IItem>());
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Once()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [HappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_the_only_similar_items_in_inventory_have_full_stack_will_add_item()
    {
        const bool isAtCapacity = false;
        var existingItemMock = MockItemData.GetItemMock(true, 8, 100);
        existingItemMock
            .Setup(item => item.CanBeStackedOn)
            .Returns(false);
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);
        _items.Setup(items => items.Values)
            .Returns(new[] {existingItemMock.Object});
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Once()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [HappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_a_similar_item_present_with_some_empty_stacks_will_add_item()
    {
        var existingItemMock = MockItemData.GetItemMock(true, 8, 10);
        existingItemMock
            .Setup(item => item.AddToStack(It.IsAny<int>()))
            .Returns(8);
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);
        _items.Setup(items => items.Values)
            .Returns(new [] {existingItemMock.Object});
        var expectedAddItemAction = (ItemStackedAndAdded, (IItem?) null);
        
        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, It.IsAny<bool>());

        Assert.Multiple(
            () => existingItemMock.Verify(item => item.AddToStack(It.IsAny<int>()), Times.Once()) ,
            () => itemToAddMock.Verify(item => item.SetStack(It.IsAny<int>()), Times.Once()),
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Once()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_a_non_stackable_item_when_inventory_is_full_will_not_add_the_item()
    {
        const bool isAtCapacity = true;
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_a_non_stackable_item_when_item_has_already_been_added_will_not_add_the_item()
    {
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        _items
            .Setup(items => items.ContainsKey(It.IsAny<Guid>()))
            .Returns(true);
        var expectedAddItemAction = (ItemAlreadyExists, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, It.IsAny<bool>());

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_if_similar_item_is_not_present_and_inventory_is_full_will_not_add_item()
    {
        const bool isAtCapacity = true;
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);

        _items
            .Setup(items => items.Values)
            .Returns(Array.Empty<IItem>());
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_inventory_is_full_after_stacking_will_not_add_item()
    {
        const bool isAtCapacity = true;
        var existingItemMock = MockItemData.GetItemMock(true, 8, 10);
        existingItemMock
            .Setup(item => item.AddToStack(It.IsAny<int>()))
            .Returns(2);
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);
        _items
            .Setup(items => items.Values)
            .Returns(new[] {existingItemMock.Object});
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => existingItemMock.Verify(item => item.AddToStack(It.IsAny<int>()), Times.Once()),
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_a_similar_item_with_enough_available_stacks_is_in_inventory_will_not_add_item()
    {
        var existingItemMock = MockItemData.GetItemMock(true, 8, 100);
        existingItemMock
            .Setup(item => item.AddToStack(10))
            .Returns(0);
        var itemToAddMock = MockItemData.GetItemMock(true, 10, 30);
        _items
            .Setup(items => items.Values)
            .Returns(new[] {existingItemMock.Object});
        var expectedAddItemAction = (ItemStacked, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_items.Object, itemToAddMock.Object, It.IsAny<bool>());

        Assert.Multiple(
            () => existingItemMock.Verify(item => item.AddToStack(It.IsAny<int>()), Times.Once()),
            () => _items.Verify(items => items.Add(It.IsAny<Guid>(), It.IsAny<IItem>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
}