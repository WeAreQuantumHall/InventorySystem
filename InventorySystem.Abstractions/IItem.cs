using System;

namespace InventorySystem.Abstractions
{
    public interface IItem
    {
        public Guid Id { get; }
        public string Identifier { get; }
        public string Name { get; }
        public bool Stackable { get; }
        public int CurrentAmount { get; }
        public int MaxAmount { get; }

        public bool IsAtMaxAmount { get; }
    }
}