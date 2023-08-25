using System.Collections.Generic;

namespace InventorySystem.Abstractions.Tags
{
    public interface ITagList : IEnumerable<ITag> 
    {
        IReadOnlyList<ITag> Tags { get; }
        bool AddTag(ITag tag);
        bool RemoveTag(ITag tag);
        bool ContainsTag(ITag tag);
    }
}