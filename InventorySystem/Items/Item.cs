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
        public Item(
            string name, 
            ITagList? tagList = null,
            IItemStack? itemStack = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            TagList = tagList ?? new TagList();
            ItemStack = itemStack;
        }
        
        public Guid Id { get; }
        public string Name { get; }
        public IItemStack? ItemStack { get; set;  }
        public ITagList TagList { get; }

        public bool AddTag(ITag tag) => TagList.AddTag(tag);
        public bool RemoveTag(ITag tag) => TagList.RemoveTag(tag);
        public bool ContainsTag(ITag tag) => TagList.ContainsTag(tag);
        
        public IItem Copy()
        {
            var tagList = new TagList(TagList.Tags);
            var itemStack = ItemStack == null
                ? null
                : new ItemStack(ItemStack.Max, ItemStack.Current);

            return new Item(Name, tagList)
            {
                ItemStack = itemStack
            };
        }
    }
}