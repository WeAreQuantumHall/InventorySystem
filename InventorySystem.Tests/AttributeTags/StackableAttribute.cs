using System;

namespace InventorySystem.Tests.AttributeTags;

[AttributeUsage(AttributeTargets.Method)]
public class StackableAttribute : Attribute { }