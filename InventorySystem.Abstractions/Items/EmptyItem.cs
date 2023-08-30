using System;
using System.Diagnostics.CodeAnalysis;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Abstractions.Items
{
    /// <summary>
    /// Is an item with no functionality, used to populate empty slots in an equipment inventory
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class EmptyItem : IItem
    {
        public string Name => "empty-item";
        public IItemStack? ItemStack => null;
        public IItem Copy() => default!;
        public ITagList TagList => default!;
        public Guid Id => default;
        public bool AddTag(ITag tag) => default;
        public bool RemoveTag(ITag tag) => default;
        public bool ContainsTag(ITag tag) => default;
    }
}