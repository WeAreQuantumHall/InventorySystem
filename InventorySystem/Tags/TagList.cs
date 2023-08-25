﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Tags
{
    internal class TagList : ITagList
    {
        private readonly List<ITag> _tagCollection;
        public IReadOnlyList<ITag> Tags => _tagCollection;

        public TagList(IEnumerable<ITag> tags) => _tagCollection = tags.ToList();

        public TagList() => _tagCollection = new List<ITag>();

        public bool AddTag(ITag tag)
        {
            if (ContainsTag(tag)) return false;

            _tagCollection.Add(tag);
            return true;
        }

        public bool RemoveTag(ITag tag)
            => _tagCollection.Remove(tag);

        public bool ContainsTag(ITag tag)
            => _tagCollection.Exists(t => t == tag);

        public IEnumerator<ITag> GetEnumerator() => _tagCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}