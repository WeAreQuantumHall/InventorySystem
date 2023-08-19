using System;
using InventorySystem.Abstractions;

namespace InventorySystem.Tests.Stubs;

public class ItemStub : IItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Identifier { get; } = "TEST_ITEM_IDENTIFIER";
    public string Name { get; } = "TEST_ITEM_NAME";
    public bool Stackable { get; } 
    public int CurrentAmount { get; }
    public int MaxAmount { get; }
    public bool IsAtMaxAmount { get; }
}