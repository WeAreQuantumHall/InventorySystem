using System;
using System.Collections.Generic;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.ActionResults;
using InventorySystem.Inventories;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;
using static InventorySystem.Abstractions.Enums.InventoryAction;


namespace InventorySystem.Tests.Inventories;

public class ContainerInventoryTests
{
   private readonly Mock<IInventoryService> _inventoryServiceMock = new Mock<IInventoryService>();
    private const string InventoryName = "test-inventory-name";

    [Constructor]
    public void Creating_a_new_container_inventory_correctly_sets_values()
    {
        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        
        Assert.Multiple(
            () => Assert.Equal(InventoryName, inventory.Name),
            () => Assert.Equal(0, inventory.Capacity));
    }

    [HappyPath]
    public void Setting_the_name_of_the_inventory_will_change_the_name()
    {
        const string newInventoryName = "new-test-inventory-name";
        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        inventory.SetName(newInventoryName);
        
        Assert.Equal(newInventoryName, inventory.Name);
    }
    
    [HappyPath]
    public void Trying_to_add_an_item_which_can_be_added_will_add_the_item_and_not_provide_an_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<IItem>(),
                It.IsAny<bool>()))
            .Returns((ItemAdded, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemAdded);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_add_an_item_to_a_full_inventory_will_not_add_the_item_and_will_provide_the_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<IItem>(),
                It.IsAny<bool>()))
            .Returns((InventoryAtCapacity, itemMock.Object));
        var expectedInventoryActionResult = new InventoryActionResult(InventoryAtCapacity, itemMock.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_add_the_same_item_twice_will_not_add_the_item_and_will_not_provide_the_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<IItem>(),
                It.IsAny<bool>()))
            .Returns((ItemAlreadyExists, itemMock.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemAlreadyExists, itemMock.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath, Stackable]
    public void Trying_to_add_an_item_which_can_fully_stack_will_not_add_the_item_and_will_not_provide_an_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<IItem>(),
                It.IsAny<bool>()))
            .Returns((ItemStacked, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemStacked);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_add_an_item_which_can_partially_stack_will_add_the_item_and_not_provide_an_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryAddItem(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<IItem>(),
                It.IsAny<bool>()))
            .Returns((ItemStackedAndAdded, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemStackedAndAdded);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryAddItem(itemMock.Object);

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_get_an_item_when_the_item_is_in_the_inventory_will_provide_the_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryGetItem(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<Guid>()))
            .Returns((ItemRetrieved, itemMock.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemRetrieved, itemMock.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetItem(It.IsAny<Guid>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_get_an_item_when_the_item_is_not_in_the_inventory_will_not_provide_the_item()
    {
        _inventoryServiceMock
            .Setup(service => service.TryGetItem(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<Guid>()))
            .Returns((ItemNotFound, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemNotFound);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetItem(It.IsAny<Guid>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_get_all_items_when_the_inventory_contains_items_will_provide_list_of_those_items()
    {
        var itemsMock = new Mock<IEnumerable<IItem>>();
        _inventoryServiceMock
            .Setup(service => service.TryGetAllItems(It.IsAny<IDictionary<Guid, IItem>>()))
            .Returns((ItemsRetrieved, itemsMock.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemsRetrieved, itemsMock.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetAllItems();

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_get_all_items_when_the_inventory_is_empty_will_not_provide_any_items()
    {
        _inventoryServiceMock
            .Setup(service => service.TryGetAllItems(It.IsAny<IDictionary<Guid, IItem>>()))
            .Returns((ItemsNotFound, Array.Empty<IItem>()));
        var expectedInventoryActionResult = new InventoryActionResult(ItemsNotFound);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetAllItems();

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_get_items_by_tag_when_the_inventory_contains_items_with_that_tag_will_provide_those_items()
    {
        var mockItems = new Mock<IEnumerable<IItem>>();
        _inventoryServiceMock
            .Setup(service => service.TryGetItemsByTag(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<ITag>()))
            .Returns((ItemsRetrieved, mockItems.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemsRetrieved, mockItems.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetItemsByTag(It.IsAny<ITag>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void
        Trying_to_get_items_by_tag_when_the_inventory_does_not_contain_items_with_that_tag_will_not_provide_items()
    {
        _inventoryServiceMock
            .Setup(service => service.TryGetItemsByTag(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<ITag>()))
            .Returns((ItemsNotFound, Array.Empty<IItem>()));
        var expectedInventoryActionResult = new InventoryActionResult(ItemsNotFound);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryGetItemsByTag(It.IsAny<ITag>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void
        Trying_to_remove_an_item_when_the_inventory_contains_the_item_will_remove_the_item_and_provides_that_item()
    {
        var itemMock = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TryRemoveItem(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<Guid>()))
            .Returns((ItemRemoved, itemMock.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemRemoved, itemMock.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryRemoveItem(It.IsAny<Guid>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_remove_an_item_when_the_inventory_does_not_contain_the_item_will_not_provide_an_item()
    {
        _inventoryServiceMock
            .Setup(service => service.TryRemoveItem(It.IsAny<IDictionary<Guid, IItem>>(), It.IsAny<Guid>()))
            .Returns((ItemNotFound, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemNotFound);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TryRemoveItem(It.IsAny<Guid>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_split_an_item_stack_when_the_item_stack_can_be_split_provides_the_created_split_item()
    {
        var mockSplitItem = new Mock<IItem>();
        _inventoryServiceMock
            .Setup(service => service.TrySplitItemStack(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<Guid>(),
                It.IsAny<int>()))
            .Returns((ItemStackSplit, mockSplitItem.Object));
        var expectedInventoryActionResult = new InventoryActionResult(ItemStackSplit, mockSplitItem.Object);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TrySplitItemStack(It.IsAny<Guid>(), It.IsAny<int>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [UnhappyPath]
    public void Trying_to_split_an_item_stack_when_the_inventory_does_not_contain_the_item_does_not_provide_an_item()
    {
        _inventoryServiceMock
            .Setup(service => service.TrySplitItemStack(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<Guid>(),
                It.IsAny<int>()))
            .Returns((ItemNotFound, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemNotFound);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TrySplitItemStack(It.IsAny<Guid>(), It.IsAny<int>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }

    [HappyPath]
    public void Trying_to_split_an_item_stack_when_the_item_stack_cannot_be_split_does_not_provide_an_item()
    {
        _inventoryServiceMock
            .Setup(service => service.TrySplitItemStack(
                It.IsAny<IDictionary<Guid, IItem>>(),
                It.IsAny<Guid>(),
                It.IsAny<int>()))
            .Returns((ItemStackNotSplit, null));
        var expectedInventoryActionResult = new InventoryActionResult(ItemStackNotSplit);

        IInventory inventory = new ContainerInventory(_inventoryServiceMock.Object, InventoryName);
        var inventoryActionResult = inventory.TrySplitItemStack(It.IsAny<Guid>(), It.IsAny<int>());

        Assert.Equivalent(expectedInventoryActionResult, inventoryActionResult);
    }
}