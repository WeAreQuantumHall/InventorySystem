using System.Runtime.CompilerServices;

namespace InventorySystem.Tests.AttributeTags;

public class ConstructorAttribute : PathAttribute
{
    public ConstructorAttribute([CallerMemberName] string propertyName = "") : base("🛠", propertyName) { }
}
