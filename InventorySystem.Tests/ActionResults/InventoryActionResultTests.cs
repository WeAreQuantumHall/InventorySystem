using InventorySystem.Abstractions.Enums;
using InventorySystem.Abstractions.Inventories;
using InventorySystem.Abstractions.Items;
using InventorySystem.ActionResults;
using Moq;
using Xunit;

namespace InventorySystem.Tests.ActionResults;

public class InventoryActionResultTests
{
    [Fact]
    public void New__ReturnsCorrectlyPopulatedValues()
    {
        const InventoryAction result = InventoryAction.ItemAdded;
        var item = new Mock<IItem>();
        
        IInventoryActionResult actionResult = new InventoryActionResult(result, item.Object);

        Assert.Multiple(
            () => Assert.Equal(result, actionResult.Result),
            () => Assert.Equal(item.Object, actionResult.Item));
    }
}