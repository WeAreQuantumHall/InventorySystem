using System;
using System.Runtime.CompilerServices;
using InventorySystem.Abstractions.Items;
using InventorySystem.Abstractions.Tags;
using InventorySystem.Tags;

[assembly: InternalsVisibleTo("InventorySystem.Tests")]
namespace InventorySystem.Items
{
    internal class Item : IItem
    {
        public Item(string name, ITagList? tagList = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            TagList = tagList ?? new TagList();
        }
        
        public static IItem Empty = new Item("empty-item"); 

        /// <inheritdoc />
        public Guid Id { get; }
        
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Stackable { get; set; }

        /// <inheritdoc />
        public int Stack { get; set; }

        /// <inheritdoc />
        public int MaxStack { get; set; }

        /// <inheritdoc />
        public bool CanBeStackedOn => Stackable && Stack < MaxStack;

        public ITagList TagList { get; }

        /// <inheritdoc />
        public int AddToStack(int amount)
        {
            var totalStack = Stack + amount;
            if (totalStack > MaxStack)
            {
                Stack = MaxStack;
                return totalStack - MaxStack;
            }

            Stack = totalStack;
            return 0;
        }


        
        /// <inheritdoc />
        public IItem SplitStack(int splitAmount)
        {
            if (!Stackable || Stack == 1 || splitAmount >= Stack) return this;

            var splitItem = new Item(Name, new TagList(TagList.Tags))
            {
                Stackable = Stackable,
                Stack = splitAmount,
                MaxStack = MaxStack
            };
            Stack -= splitAmount;

            return splitItem;
        }

        public bool AddTag(ITag tag) => TagList.AddTag(tag);
        
        public bool RemoveTag(ITag tag) => TagList.RemoveTag(tag);
        
        public bool ContainsTag(ITag tag) => TagList.ContainsTag(tag);
    }
}