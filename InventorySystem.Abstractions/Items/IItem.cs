using System;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Abstractions.Items
{
    public interface IItem : ITagMember
    {
        public static readonly IItem Empty = new EmptyItem();
        Guid Id { get; }
        string Name { get; }
        IItemStack? ItemStack { get; }
        public IItem Copy();
    }
}