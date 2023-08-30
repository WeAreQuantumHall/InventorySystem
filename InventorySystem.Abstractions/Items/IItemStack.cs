namespace InventorySystem.Abstractions.Items
{
    public interface IItemStack
    {
        int Current { get; }
        int Max { get; }
        bool CanBeStackedOn { get; }
        int AddToStack(int amount);
        int SetStack(int amount);
        bool TrySplitStack(int splitAmount);
    }
}