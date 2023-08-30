using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Services.Inventory;
using InventorySystem.Tags;
using InventorySystem.Tests.AttributeTags;
using InventorySystem.Tests.Data;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;

namespace InventorySystem.Tests.Services.Inventory;

public class EquipmentInventoryServiceTests
{
    private readonly IDictionary<Guid, IItem> _itemsStub = EquipmentTag.Tags
        .ToDictionary(tag => tag.Identifier, _ => IItem.Empty);

    private readonly Mock<ITagList> _tagListMock = new Mock<ITagList>();
    private readonly Mock<IItemStack> _itemStackMock = new Mock<IItemStack>();

    [HappyPath]
    public void Trying_to_add_an_item_with_empty_matching_slot_will_add_the_item_and_not_provide_an_item()
    {
        const bool isAtCapacity = false;
        _tagListMock
            .Setup(tagList => tagList.Tags)
            .Returns(new [] {EquipmentTag.Head});
        var itemToAddMock = MockItemData.GetItemMock(_tagListMock);
        var expectedAddItemAction = (ItemAdded, (IItem?) null);

        IInventoryService inventoryService = new EquipmentInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.Contains(itemToAddMock.Object, _itemsStub.Values),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [HappyPath]
    public void Trying_to_add_an_item_without_empty_matching_slot_will_add_the_item_and_provide_the_item_in_the_slot()
    {
        const bool isAtCapacity = false;
        var identifier = EquipmentTag.Head.Identifier;
        var itemInSlotMock = MockItemData.GetItemMock(_tagListMock);
        _itemsStub[identifier] = itemInSlotMock.Object;
        _tagListMock
            .Setup(tagList => tagList.Tags)
            .Returns(new [] {EquipmentTag.Head});
        var itemToAddMock = MockItemData.GetItemMock(_tagListMock);
        
        var expectedAddItemAction = (ItemSwapped, itemInSlotMock.Object);
    
        IInventoryService inventoryService = new EquipmentInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);
    
        Assert.Multiple(
            () => Assert.Contains(itemToAddMock.Object, _itemsStub.Values),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath, Stackable]
    public void Trying_to_add_a_stackable_item_will_not_add_the_item_and_will_provide_the_item()
    {
        const bool isAtCapacity = false;
        var itemToAddMock = MockItemData.GetItemMock(_tagListMock, _itemStackMock);
        var expectedAddItemAction = (StackableItemsNotAllowed, itemToAddMock.Object);

        IInventoryService inventoryService = new EquipmentInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);

        Assert.Multiple(
            () => Assert.All(_itemsStub, item => Assert.Equal(item.Value, IItem.Empty)),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }

    [UnhappyPath]
    public void Trying_to_add_an_item_without_equipment_tags_will_not_add_the_item_and_will_provide_the_item()
    {
        const bool isAtCapacity = false;
        _tagListMock
            .Setup(tagList => tagList.Tags)
            .Returns(Array.Empty<ITag>());
        var itemToAddMock = MockItemData.GetItemMock(_tagListMock);
        
        var expectedAddItemAction = (ItemEquipmentTagMissing, itemToAddMock.Object);
    
        IInventoryService inventoryService = new EquipmentInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);
    
        Assert.Multiple(
            () => Assert.All(_itemsStub, item => Assert.Equal(item.Value, IItem.Empty)),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_add_an_item_without_matching_equipment_tag_will_not_add_the_item_and_will_provide_the_item()
    {
        const bool isAtCapacity = false;
        _itemsStub.Remove(EquipmentTag.Head.Identifier);
        _tagListMock
            .Setup(tagList => tagList.Tags)
            .Returns(new [] {EquipmentTag.Head});
        var itemToAddMock = MockItemData.GetItemMock(_tagListMock);

        var expectedAddItemAction = (NoMatchingEquipmentSlots, itemToAddMock.Object);
    
        IInventoryService inventoryService = new EquipmentInventoryService();
        var addItemAction = inventoryService.TryAddItem(_itemsStub, itemToAddMock.Object, isAtCapacity);
    
        Assert.Multiple(
            () => Assert.All(_itemsStub, item => Assert.Equal(item.Value, IItem.Empty)),
            () => Assert.Equivalent(expectedAddItemAction, addItemAction));
    }
    
    [HappyPath]
    public void Trying_to_get_an_item_when_the_slot_and_item_are_present_will_provide_the_item()
    {
        var identifier = EquipmentTag.Head.Identifier;
        var itemInSlotMock = MockItemData.GetItemMock();
        _itemsStub[identifier] = itemInSlotMock.Object;
        var expectedGetItemAction = (ItemRetrieved, itemInSlotMock.Object);
        
        IInventoryService inventoryService = new EquipmentInventoryService();
        var getItemAction = inventoryService.TryGetItem(_itemsStub, identifier);
    
        Assert.Equivalent(expectedGetItemAction, getItemAction);
    }
    
    [UnhappyPath]
    public void Trying_to_get_an_item_when_the_slot_is_not_present_will_not_provide_an_item()
    {
        _itemsStub.Clear();
        var expectedGetItemAction = (ItemNotFound, (IItem?) null);
        IInventoryService inventoryService = new EquipmentInventoryService();
        var getItemAction = inventoryService.TryGetItem(_itemsStub, Guid.NewGuid());
        
        Assert.Equivalent(expectedGetItemAction, getItemAction);
    }
    
    [UnhappyPath]
    public void Trying_to_get_an_item_when_the_slot_is_present_but_does_not_contain_an_item_will_not_provide_an_item()
    {
        var identifier = EquipmentTag.Head.Identifier;
        _itemsStub[identifier] = IItem.Empty;
        var expectedGetItemAction = (ItemNotFound, (IItem?) null);
        
        IInventoryService inventoryService = new EquipmentInventoryService();
        var getItemAction = inventoryService.TryGetItem(_itemsStub, identifier);
    
        Assert.Equivalent(expectedGetItemAction, getItemAction);
    }

    [HappyPath]
    public void Trying_to_get_all_items_when_items_are_present_will_return_those_items()
    {
        var firstItemToGetMock = MockItemData.GetItemMock();
        var secondItemToGetMock = MockItemData.GetItemMock();
        _itemsStub[EquipmentTag.Head.Identifier] = firstItemToGetMock.Object;
        _itemsStub[EquipmentTag.Chest.Identifier] = secondItemToGetMock.Object;
        var expectedList = new List<IItem> {firstItemToGetMock.Object, secondItemToGetMock.Object};

        IInventoryService inventoryService = new EquipmentInventoryService();
        var getAllItemsAction = inventoryService.TryGetAllItems(_itemsStub);

        Assert.Multiple(
            () => Assert.Equal(ItemsRetrieved, getAllItemsAction.Action),
            () => Assert.Equal(expectedList, getAllItemsAction.Items));
    }
    

    [UnhappyPath]
    public void Trying_to_get_all_items_when_inventory_contains_no_slots_will_not_return_items()
    {
        IInventoryService inventoryService = new EquipmentInventoryService();
        var getAllItemsAction = inventoryService.TryGetAllItems(_itemsStub);
        _itemsStub.Clear();

        Assert.Multiple(
            () => Assert.Equal(ItemsNotFound, getAllItemsAction.Action),
            () => Assert.Empty(getAllItemsAction.Items));
    }
    
    [UnhappyPath]
    public void Trying_to_get_all_items_when_all_slots_are_empty_will_not_return_items()
    {
        IInventoryService inventoryService = new EquipmentInventoryService();
        var getAllItemsAction = inventoryService.TryGetAllItems(_itemsStub);

        Assert.Multiple(
            () => Assert.Equal(ItemsNotFound, getAllItemsAction.Action),
            () => Assert.Empty(getAllItemsAction.Items));
    }
    
    [HappyPath]
    public void Trying_to_remove_an_item_when_the_slot_is_present_and_contains_an_item_will_remove_and_provide_that_item()
    {
        var itemToRemove = MockItemData.GetItemMock();
        _itemsStub[EquipmentTag.Head.Identifier] = itemToRemove.Object;
        var expectedRemoveItemAction = (ItemRemoved, itemToRemove.Object);
        
        IInventoryService inventoryService = new EquipmentInventoryService();
        var removeItemAction = inventoryService.TryRemoveItem(_itemsStub, EquipmentTag.Head.Identifier);

        Assert.Multiple(
            () => Assert.Equal(IItem.Empty, _itemsStub[EquipmentTag.Head.Identifier]),
            () => Assert.Equivalent(expectedRemoveItemAction, removeItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_remove_an_item_when_the_slot_is_present_and_does_not_contain_an_item_will_not_provide_an_item()
    {
        var expectedRemoveItemAction = (ItemNotFound, (IItem?) null);
        
        IInventoryService inventoryService = new EquipmentInventoryService();
        var removeItemAction = inventoryService.TryRemoveItem(_itemsStub, EquipmentTag.Head.Identifier);

        Assert.Multiple(
            () => Assert.Equal(IItem.Empty, _itemsStub[EquipmentTag.Head.Identifier]),
            () => Assert.Equivalent(expectedRemoveItemAction, removeItemAction));
    }
    
    [UnhappyPath]
    public void Trying_to_remove_an_item_when_the_slot_is_not_present_will_not_provide_an_item()
    {
        _itemsStub.Remove(EquipmentTag.Head.Identifier);
        var expectedRemoveItemAction = (ItemNotFound, (IItem?) null);

        IInventoryService inventoryService = new EquipmentInventoryService();
        var removeItemAction = inventoryService.TryRemoveItem(_itemsStub, EquipmentTag.Head.Identifier);

        Assert.Equivalent(expectedRemoveItemAction, removeItemAction);
    }

    [HappyPath]
    public void Trying_to_split_an_item_stack_on_an_item_is_not_allowed()
    {
        var expectedRemoveItemAction = (StackableItemsNotAllowed, (IItem?) null);

        IInventoryService inventoryService = new EquipmentInventoryService();
        var removeItemAction = inventoryService.TrySplitItemStack(_itemsStub, It.IsAny<Guid>(), It.IsAny<int>());

        Assert.Equivalent(expectedRemoveItemAction, removeItemAction);
    }
}