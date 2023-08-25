using System;

namespace InventorySystem.Abstractions.Tags
{
    public interface ITag
    {
        Guid Identifier { get; }
        string Name { get; }
    }
 
}