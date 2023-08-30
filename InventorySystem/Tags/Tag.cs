using System;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Tags
{
    public class Tag : ITag
    {
        public Tag(string name, Guid identifier)
        {
            Name = name;
            Identifier = identifier;
        }
        
        public Guid Identifier { get; }
        public string Name { get; }

        public override string ToString() => Name;
    }
}