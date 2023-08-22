using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;

namespace InventorySystem.Tags
{
    internal class TagList : ITagList
    {
        private List<string> ListOfTags { get; }
        
        public IReadOnlyList<string> Tags => ListOfTags;

        public TagList(IEnumerable<string> tags)
            => ListOfTags = tags.ToList();

        public TagList() 
            => ListOfTags = new List<string>();

        public bool AddTag(string tag)
        {
            if (IsMember(tag)) return false;

            ListOfTags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag) 
            => ListOfTags.Remove(tag);

        public bool IsMember(string tag) 
            => ListOfTags.Exists(t => t == tag);
    }
}