using InventorySystem.Abstractions.Items;

namespace InventorySystem.Items
{
    internal class ItemStack : IItemStack
    {
        public ItemStack(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public int Current { get; private set; }
        public int Max { get; }
        
        public bool CanBeStackedOn => Current < Max;
        
        public int AddToStack(int amount)
        {
            var totalStack = Current + amount;
            if (totalStack > Max)
            {
                Current = Max;
                return totalStack - Max;
            }

            Current = totalStack;
            return 0;
        }
        
        public int SetStack(int amount)
        {
            if (amount > Max)
            {
                Current = Max;
                return amount - Max;
            }

            Current = amount;
            return 0;
        }
        
        public bool TrySplitStack(int splitAmount)
        {
            if (Current == 1 || splitAmount >= Max) return false;
            Current -= splitAmount;
            return true;
        }
    }
}