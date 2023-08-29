using System.Runtime.CompilerServices;

namespace InventorySystem.Tests.AttributeTags;

public sealed class UnhappyPathAttribute : PathAttribute
{
    public UnhappyPathAttribute([CallerMemberName] string propertyName = "") : base("👎", propertyName) { }
}