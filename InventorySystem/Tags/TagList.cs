using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions;

namespace InventorySystem.Tags
{
    internal class TagList : ITagList
    {
        private List<string> TagCollection { get; }
        
        public IReadOnlyList<string> Tags => TagCollection;

        public TagList(IEnumerable<string> tags) => TagCollection = tags.ToList();

        public TagList() => TagCollection = new List<string>();

        public bool AddTag(string tag)
        {
            if (ContainsTag(tag)) return false;

            TagCollection.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag) => TagCollection.Remove(tag);

        public bool ContainsTag(string tag)
        {
            var x = TagCollection.Exists(t => t == tag);
            return x;
        } 
    }
}