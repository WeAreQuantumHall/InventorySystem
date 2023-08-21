using System;
using System.Runtime.CompilerServices;
using InventorySystem.Abstractions;
using InventorySystem.Abstractions.Enums;

[assembly: InternalsVisibleTo("InventorySystem.Tests")]
namespace InventorySystem
{
    /// <inheritdoc />
    internal class Item : IItem
    {

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Identifier { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool Stackable { get; set; }

        /// <inheritdoc />
        public int Stack { get; set; }

        /// <inheritdoc />
        public int MaxStack { get; set; }

        /// <inheritdoc />
        public bool CanBeStackedOn => Stackable && Stack < MaxStack;

        /// <inheritdoc />
        /// <remarks>Defaults to <see cref="EquipmentCategory.None"/></remarks>
        public EquipmentCategory EquipmentCategory { get; set; } = EquipmentCategory.None;

        /// <inheritdoc />
        public int AddToStack(int amount)
        {
            var totalStack = Stack + amount;
            if (totalStack > MaxStack)
            {
                Stack = MaxStack;
                return totalStack - MaxStack;
            }

            Stack = totalStack;
            return 0;
        } 
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with a specified identifier and name.
        /// </summary>
        /// <param name="identifier">The identifier of the item.</param>
        /// <param name="name">The name of the item.</param>
        public Item(string identifier, string name)
        {
            Id = Guid.NewGuid();
            Identifier = identifier;
            Name = name;
        }
        
        /// <inheritdoc />
        public IItem SplitStack(int splitAmount)
        {
            if (!Stackable || Stack == 1 || splitAmount >= Stack) return this;

            var splitItem = new Item(Identifier, Name)
            {
                Stackable = Stackable,
                Stack = splitAmount,
                MaxStack = MaxStack
            };
            Stack -= splitAmount;

            return splitItem;
        }
    }
}