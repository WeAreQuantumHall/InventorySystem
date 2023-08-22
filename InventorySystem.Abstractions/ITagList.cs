using System.Collections.Generic;

namespace InventorySystem.Abstractions
{
    public interface ITagList
    {
        IReadOnlyList<string> Tags { get; }
        bool AddTag(string tag);
        bool RemoveTag(string tag);
        bool IsMember(string tag);
    }
}