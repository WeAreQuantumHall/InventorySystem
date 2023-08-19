using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InventorySystem.Abstractions;

namespace InventorySystem
{
    public class Inventory
    {
        public Guid Id { get; }
        public string Name { get; private set; }

        private Dictionary<Guid, IItem> Items { get; }

        public Inventory(string name)
        {
            Id = Guid.NewGuid();
            Items = new Dictionary<Guid, IItem>();
            Name = name;
        }

        public void SetName(string name) => Name = name;
        
        
        public bool TryAddItem(IItem item) 
            => Items.TryAdd(item.Id, item);

        public bool TryGetItem(Guid id, [MaybeNullWhen(false)] out IItem readOnlyItem)
        {
            if (Items.TryGetValue(id, out var item))
            {
                readOnlyItem = item;
                return true;
            }

            readOnlyItem = null;
            return false;
        }
            



    }
}