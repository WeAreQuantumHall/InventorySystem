using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Services.Inventory;
using InventorySystem.Tests.AttributeTags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Services.Inventory;

public class InventoryServiceTests
{
    private readonly IDictionary<Guid, IItem> _itemsStub = new Dictionary<Guid, IItem>();

    [HappyPath]
    public void Trying_to_add_an_item_which_has_not_already_been_added_will_add_the_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new InventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.Contains(itemToAddMock.Object, _itemsStub.Values),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    [ExcludeFromCodeCoverage]
    public void Trying_to_add_an_item_when_inventory_is_full_will_not_add_the_item()
    {
        const bool isAtCapacity = true;
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        var expectedAddItemAction = (InventoryAtCapacity, itemToAddMock.Object);

        IInventoryService inventoryService = new InventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.DoesNotContain(_itemsStub, items => items.Key == itemToAddMock.Object.Id),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_an_item_when_item_has_already_been_added_will_not_add_the_item()
    {
        var itemToAddMock = MockItemData.GetItemMock(false, 1, 1);
        _itemsStub.Add(itemToAddMock.Object.Id, itemToAddMock.Object);
        var expectedAddItemAction = (ItemAlreadyExists, (IItem?) null);

        IInventoryService inventoryService = new InventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, It.IsAny<bool>());

        Assert.Multiple(
            () => Assert.Single(_itemsStub.Values, itemToAddMock.Object),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [HappyPath]
    public void Trying_to_get_an_item_if_the_item_is_in_the_inventory_will_provide_item()
    {
        var existingItemMock = MockItemData.GetItemMock(false, 1, 1);
        _itemsStub.Add(existingItemMock.Object.Id, existingItemMock.Object);
        var expectedGetItemAction = (ItemRetrieved, existingItemMock.Object);
        
        IInventoryService inventoryService = new InventoryService();
        var getItemAction = inventoryService.TryGetItem(_itemsStub, existingItemMock.Object.Id);

        Assert.Equivalent(expectedGetItemAction, getItemAction);
    }
    
    [UnhappyPath]
    public void Trying_to_get_an_item_if_the_item_is_not_in_inventory_will_not_provide_item()
    {
        IInventoryService inventoryService = new InventoryService();
        var expectedGetItemAction = (ItemNotFound, (IItem?) null);
        
        var getItemAction = inventoryService.TryGetItem(_itemsStub, It.IsAny<Guid>());

        Assert.Equivalent(expectedGetItemAction, getItemAction);
    }

    [HappyPath]
    public void Trying_to_get_all_items_if_the_inventory_contains_items_will_provide_items()
    {
        var firstItemMock = MockItemData.GetItemMock(false);
        var secondItemMock = MockItemData.GetItemMock(false);
        var itemsList = new List<IItem> {firstItemMock.Object, secondItemMock.Object};
        _itemsStub.Add(firstItemMock.Object.Id, firstItemMock.Object);
        _itemsStub.Add(secondItemMock.Object.Id, secondItemMock.Object);

        IInventoryService inventoryService = new InventoryService();
        var getAllItemsAction = inventoryService.TryGetAllItems(_itemsStub);

        Assert.Multiple(
            () => Assert.Equal(ItemsRetrieved, getAllItemsAction.Action),
            () => Assert.Equal(itemsList, getAllItemsAction.Items));
    }
    
    [HappyPath]
    public void Trying_to_get_all_items_if_the_inventory_is_empty_will_not_provide_any_items()
    {
        IInventoryService inventoryService = new InventoryService();
        
        var getAllItemsAction = inventoryService.TryGetAllItems(_itemsStub);
        
        Assert.Multiple(
            () => Assert.Equal(ItemsNotFound, getAllItemsAction.Action),
            () => Assert.Equal(Array.Empty<IItem>(), getAllItemsAction.Items));
    }

    [HappyPath]
    public void Trying_to_get_items_by_tag_when_inventory_contains_items_with_that_tag_will_provide_those_items()
    {
        var mockItemWithTag = new Mock<IItem>();
        mockItemWithTag
            .Setup(item => item.ContainsTag(It.IsAny<ITag>()))
            .Returns(true);
        _itemsStub.Add(mockItemWithTag.Object.Id, mockItemWithTag.Object);
        var itemsList = new List<IItem> {mockItemWithTag.Object};
        
        IInventoryService inventoryService = new InventoryService();
        
        var getItemsByTagAction = inventoryService.TryGetItemsByTag(_itemsStub, It.IsAny<ITag>());

        Assert.Multiple(
            () => mockItemWithTag.Verify(item => item.ContainsTag(It.IsAny<ITag>()), Times.Once),
            () => Assert.Equal(ItemsRetrieved, getItemsByTagAction.Action),
            () => Assert.Equal(itemsList, getItemsByTagAction.Items));
    }
    
    [UnhappyPath]
    public void Trying_to_get_items_by_tag_when_inventory_does_not_contain_items_with_that_tag_will_not_provide_items()
    {
        var mockItemWithoutTag = new Mock<IItem>();
        mockItemWithoutTag
            .Setup(item => item.ContainsTag(It.IsAny<ITag>()))
            .Returns(false);
        _itemsStub.Add(mockItemWithoutTag.Object.Id, mockItemWithoutTag.Object);
        
        IInventoryService inventoryService = new InventoryService();
        var getItemsByTagAction = inventoryService.TryGetItemsByTag(_itemsStub, It.IsAny<ITag>());
        
        Assert.Multiple(
            () => mockItemWithoutTag.Verify(item => item.ContainsTag(It.IsAny<ITag>()), Times.Once),
            () => Assert.Equal(ItemsNotFound, getItemsByTagAction.Action),
            () => Assert.Equal(Array.Empty<IItem>(), getItemsByTagAction.Items));
    }
    
    [HappyPath]
    public void Trying_to_remove_an_item_when_the_inventory_contains_the_item_will_remove_and_provide_that_item()
    {
        var mockItemToRemove = MockItemData.GetItemMock(false);
        _itemsStub.Add(mockItemToRemove.Object.Id, mockItemToRemove.Object);
        
        IInventoryService inventoryService = new InventoryService();
        var expectedRemoveItemAction = (ItemRemoved, mockItemToRemove.Object);
        
        var removeItemAction = inventoryService.TryRemoveItem(_itemsStub, mockItemToRemove.Object.Id);
    
        Assert.Multiple(
            () => Assert.Empty(_itemsStub),
            () => Assert.Equal(expectedRemoveItemAction, removeItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_remove_an_item_when_the_inventory_does_not_contain_the_item_will_not_provide_that_item()
    {
        var itemToRemoveId = Guid.NewGuid();
        var expectedRemoveItemAction = (ItemNotFound, (IItem?) null);
        
        IInventoryService inventoryService = new InventoryService();
        var removeItemAction = inventoryService.TryRemoveItem(_itemsStub, itemToRemoveId);

        Assert.Equivalent(expectedRemoveItemAction, removeItemAction);
    }
    
    [HappyPath]
    public void Trying_to_split_an_item_stack_when_it_can_be_split_will_provide_a_new_item()
    {
        const int splitAmount = 5;
        var itemToSplitMock = MockItemData.GetItemMock(true, 10, 10);
        var itemFromSplittingMock = MockItemData.GetItemMock(true, 5, 10);
        _itemsStub.Add(itemToSplitMock.Object.Id, itemToSplitMock.Object);
        itemToSplitMock
            .Setup(item => item.SplitStack(splitAmount))
            .Returns(itemFromSplittingMock.Object);
        var expectedRemoveItemAction = (ItemStackSplit, itemFromSplittingMock.Object);
    
        IInventoryService inventoryService = new InventoryService();
        var removeItemAction = inventoryService.TrySplitItemStack(_itemsStub, itemToSplitMock.Object.Id, splitAmount);
    
        Assert.Multiple(
            () => itemToSplitMock.Verify(item => item.SplitStack(splitAmount), Times.Once),
            () => Assert.Equivalent(expectedRemoveItemAction, removeItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_split_an_item_stack_when_item_is_not_found_will_not_provide_an_item()
    {
        var expectedRemoveItemAction = (ItemNotFound, (IItem?) null);
        
        IInventoryService inventoryService = new InventoryService();
        var removeItemAction = inventoryService.TrySplitItemStack(_itemsStub, It.IsAny<Guid>(), It.IsAny<int>());
    
        Assert.Equivalent(expectedRemoveItemAction, removeItemAction);
    }
    
    [UnhappyPath]
    public void Trying_to_split_an_item_stack_when_item_stack_cannot_be_split_will_not_provide_an_item()
    {
        var itemToSplitMock = MockItemData.GetItemMock(true, 1, 1);
        itemToSplitMock
            .Setup(item => item.SplitStack(It.IsAny<int>()))
            .Returns(itemToSplitMock.Object);
        _itemsStub.Add(itemToSplitMock.Object.Id, itemToSplitMock.Object);
        var expectedRemoveItemAction = (ItemStackNotSplit, (IItem?) null);
        
        IInventoryService inventoryService = new InventoryService();
        var removeItemAction = inventoryService.TrySplitItemStack(_itemsStub, itemToSplitMock.Object.Id, It.IsAny<int>());
    
        Assert.Multiple(
            () => itemToSplitMock.Verify(item => item.SplitStack(It.IsAny<int>()), Times.Once),
            () => Assert.Equivalent(expectedRemoveItemAction, removeItemAction));
    }
}