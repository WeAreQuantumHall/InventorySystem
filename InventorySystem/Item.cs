using System;
using InventorySystem.Abstractions;

namespace InventorySystem
{
    internal class Item : IItem
    {

        public Guid Id { get; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public bool Stackable { get; set; }
        public int CurrentAmount { get; set; }
        public int MaxAmount { get; set; }

        public bool IsAtMaxAmount => CurrentAmount >= MaxAmount;

        public Item()
        {
            Id = Guid.NewGuid();
        }
    }
}