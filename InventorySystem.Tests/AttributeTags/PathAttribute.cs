using System.Runtime.CompilerServices;
using Humanizer;
using Xunit;

namespace InventorySystem.Tests.AttributeTags;

public class PathAttribute : FactAttribute
{
    public sealed override string DisplayName { get; set; }

    protected PathAttribute(string symbol, [CallerMemberName] string propertyName = "")
    {
        DisplayName = $"[{symbol}] {propertyName.Humanize(LetterCasing.Sentence)}";
    }
}