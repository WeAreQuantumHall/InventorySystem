using System.Runtime.CompilerServices;

namespace InventorySystem.Tests.AttributeTags;

public sealed class HappyPathAttribute : PathAttribute
{
    public HappyPathAttribute([CallerMemberName] string propertyName = "") : base("👍", propertyName) { }
}