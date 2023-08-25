namespace InventorySystem.Abstractions.Tags
{
    public interface ITagMember
    {
        ITagList TagList { get; }
        bool AddTag(ITag tag);
        bool RemoveTag(ITag tag);
        bool ContainsTag(ITag tag);
    }
}