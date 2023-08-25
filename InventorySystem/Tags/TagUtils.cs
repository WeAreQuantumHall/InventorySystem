using System.Linq;
using InventorySystem.Abstractions;

namespace InventorySystem.Tags
{
    public static class TagUtils
    {
        public static readonly ITagList EquipmentTags;

        static TagUtils()
        {
            EquipmentTags = new TagList(typeof(EquipmentTag).GetFields().Select(fi => fi.Name));    
        }
    }
}