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
    private readonly IDictionary<Guid, IItem> _itemsStub = new Dictionary<Guid, IItem>();
    private readonly Mock<IItemStack> _itemStackMock = new Mock<IItemStack>();
    
    [HappyPath]
    public void Trying_to_add_a_non_stackable_item_which_has_not_already_been_added_will_add_the_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock();
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.Single(_itemsStub.Values, itemToAddMock.Object),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [HappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_similar_item_not_in_inventory_and_inventory_is_not_full_will_add_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock(itemStack: _itemStackMock);

        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.Single(_itemsStub.Values, itemToAddMock.Object),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [HappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_the_only_similar_items_in_inventory_have_full_stack_will_add_item()
    {
        const bool isAtCapacity = false;
        var existingItemMock = MockItemData.GetItemMock(itemStack: _itemStackMock);
        var itemToAddItemStackMock = new Mock<IItemStack>();
        var itemToAddMock = MockItemData.GetItemMock(itemStack: itemToAddItemStackMock);
        _itemStackMock
            .Setup(itemStack => itemStack.CanBeStackedOn)
            .Returns(false);
        _itemsStub
            .Add(existingItemMock.Object.Id, existingItemMock.Object);
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.Contains(itemToAddMock.Object, _itemsStub.Values),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [HappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_a_similar_item_present_with_some_empty_stacks_will_add_item()
    {
        const bool isAtCapacity = false;
        const int expectedRemainingStack = 5;
        var existingItemMock = MockItemData.GetItemMock(itemStack: _itemStackMock);
        var itemToAddItemStackMock = new Mock<IItemStack>();
        var itemToAddMock = MockItemData.GetItemMock(itemStack: itemToAddItemStackMock);
        _itemStackMock
            .Setup(itemStack => itemStack.CanBeStackedOn)
            .Returns(true);
        _itemStackMock
            .Setup(itemStack => itemStack.AddToStack(It.IsAny<int>()))
            .Returns(expectedRemainingStack);
        _itemsStub
            .Add(existingItemMock.Object.Id, existingItemMock.Object);
        var expectedAddItemAction = (ItemStackedAndAdded, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _itemStackMock.Verify(itemStack => itemStack.AddToStack(It.IsAny<int>()), Times.Once()),
            () => itemToAddItemStackMock.Verify(itemStack => itemStack.SetStack(expectedRemainingStack), Times.Once()),
            () => Assert.Contains(itemToAddMock.Object, _itemsStub.Values),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_a_non_stackable_item_when_inventory_is_full_will_not_add_the_item()
    {
        const bool isAtCapacity = true;
        var itemToAddMock = MockItemData.GetItemMock();
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);
    
        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);
        Assert.Multiple(
            () => Assert.Empty(_itemsStub),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_a_non_stackable_item_when_the_item_has_already_been_added_will_not_add_the_item()
    {
        var itemToAddMock = MockItemData.GetItemMock();
        _itemsStub.Add(itemToAddMock.Object.Id, itemToAddMock.Object);
            
        var expectedAddItemAction = (ItemAlreadyExists, (IItem?) null);
    
        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, It.IsAny<bool>());
    
        Assert.Multiple(
            () => Assert.Single(_itemsStub.Values, itemToAddMock.Object),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_if_similar_item_is_not_present_and_inventory_is_full_will_not_add_item()
    {
        const bool isAtCapacity = true;
        var itemToAddMock = MockItemData.GetItemMock(itemStack: _itemStackMock);
    
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);
    
        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);
    
        Assert.Multiple(
            () => Assert.Empty(_itemsStub),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_inventory_is_full_after_stacking_will_not_add_item()
    {
        const bool isAtCapacity = true;
        const int expectedRemainingStack = 5;
        var existingItemMock = MockItemData.GetItemMock(itemStack: _itemStackMock);
        var itemToAddItemStackMock = new Mock<IItemStack>();
        var itemToAddMock = MockItemData.GetItemMock(itemStack: itemToAddItemStackMock);
        _itemStackMock
            .Setup(itemStack => itemStack.CanBeStackedOn)
            .Returns(true);
        _itemStackMock
            .Setup(itemStack => itemStack.AddToStack(It.IsAny<int>()))
            .Returns(expectedRemainingStack);
        _itemsStub
            .Add(existingItemMock.Object.Id, existingItemMock.Object);
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _itemStackMock.Verify(itemStack => itemStack.AddToStack(It.IsAny<int>()), Times.Once()),
            () => itemToAddItemStackMock.Verify(itemStack => itemStack.SetStack(expectedRemainingStack), Times.Once()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_when_a_similar_item_with_enough_available_stacks_is_present_will_not_add_item()
    {
        const bool isAtCapacity = true;
        const int expectedRemainingStack = 0;
        var existingItemMock = MockItemData.GetItemMock(itemStack: _itemStackMock);
        var itemToAddItemStackMock = new Mock<IItemStack>();
        var itemToAddMock = MockItemData.GetItemMock(itemStack: itemToAddItemStackMock);
        _itemStackMock
            .Setup(itemStack => itemStack.CanBeStackedOn)
            .Returns(true);
        _itemStackMock
            .Setup(itemStack => itemStack.AddToStack(It.IsAny<int>()))
            .Returns(expectedRemainingStack);
        _itemsStub
            .Add(existingItemMock.Object.Id, existingItemMock.Object);
        var expectedAddItemAction = (ItemStacked, (IItem?) null);

        IInventoryService inventoryService = new StackableInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => _itemStackMock.Verify(itemStack => itemStack.AddToStack(It.IsAny<int>()), Times.Once()),
            () => itemToAddItemStackMock.Verify(itemStack => itemStack.SetStack(It.IsAny<int>()), Times.Never()),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
}