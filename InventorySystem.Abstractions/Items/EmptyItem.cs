using System;
using System.Diagnostics.CodeAnalysis;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Abstractions.Items
{
    [ExcludeFromCodeCoverage]
    internal class EmptyItem : IItem
    {
        public string Name => "empty-item";
        public ITagList TagList => default!;
        public bool AddTag(ITag tag) => default;
        public bool RemoveTag(ITag tag) => default;
        public bool ContainsTag(ITag tag) => default;
        public Guid Id => default;
        public bool Stackable => default;
        public int Stack => default;
        public int MaxStack => default;
        public bool CanBeStackedOn => default;
        public int AddToStack(int amount) => default;
        public int SetStack(int amount) => default;
        
        public IItem SplitStack(int splitAmount) => default!;

    }
}