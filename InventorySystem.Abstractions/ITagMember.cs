namespace InventorySystem.Abstractions
{
    public interface ITagMember
    {
        ITagList TagList { get; }
        bool AddTag(string tag);
        bool RemoveTag(string tag);
        bool ContainsTag(string tag);
    }
}