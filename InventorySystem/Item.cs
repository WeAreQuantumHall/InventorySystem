using System;
using System.Runtime.CompilerServices;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;
using InventorySystem.Tags;

[assembly: InternalsVisibleTo("InventorySystem.Tests")]
namespace InventorySystem
{
    internal class Item : IItem
    {

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Identifier { get; }

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

        /// <inheritdoc />
        public ItemCategory ItemCategory { get; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with a specified identifier and name.
        /// </summary>
        /// <param name="identifier">The identifier of the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="category"></param>
        /// <param name="tagList"></param>
        public Item(string identifier, string name, ItemCategory category, ITagList? tagList = null)
        {
            Id = Guid.NewGuid();
            Identifier = identifier;
            Name = name;
            ItemCategory = category;
            TagList = tagList ?? new TagList();
        }
        
        /// <inheritdoc />
        public IItem SplitStack(int splitAmount)
        {
            if (!Stackable || Stack == 1 || splitAmount >= Stack) return this;

            var splitItem = new Item(Identifier, Name, ItemCategory, new TagList(TagList.Tags))
            {
                Stackable = Stackable,
                Stack = splitAmount,
                MaxStack = MaxStack
            };
            Stack -= splitAmount;

            return splitItem;
        }

        public bool AddTag(string tag) => TagList.AddTag(tag);
        
        public bool RemoveTag(string tag) => TagList.RemoveTag(tag);
        
        public bool ContainsTag(string tag) => TagList.ContainsTag(tag);
    }
}