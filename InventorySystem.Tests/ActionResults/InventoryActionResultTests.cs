using System.Collections.Generic;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.ActionResults;
using InventorySystem.Tests.AttributeTags;
using Moq;
using Xunit;

namespace InventorySystem.Tests.ActionResults;

public class InventoryActionResultTests
{
    [Constructor]
    public void Creating_a_new_inventory_action_when_a_single_item_is_provided_will_correctly_set_values()
    {
        const InventoryAction result = InventoryAction.ItemAdded;
        var item = new Mock<IItem>();
        
        IInventoryActionResult actionResult = new InventoryActionResult(result, item.Object);

        Assert.Multiple(
            () => Assert.Equal(result, actionResult.Result),
            () => Assert.Equal(item.Object, actionResult.Item));
    }
    
    [Constructor]
    public void Creating_a_new_inventory_action_when_a_list_of_items_is_provided_will_correctly_set_values()
    {
        const InventoryAction result = InventoryAction.ItemAdded;
        var items = new List<IItem> {new Mock<IItem>().Object};
        
        IInventoryActionResult actionResult = new InventoryActionResult(result, items);

        Assert.Multiple(
            () => Assert.Equal(result, actionResult.Result),
            () => Assert.Equal(items, actionResult.Items));
    }
}