using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Tags
{
    public static class EquipmentTag
    {
        private static readonly ITagList TagList;

        public static readonly ITag Head = new Tag(nameof(Head), GenerateGuid(1));
        public static readonly ITag Chest = new Tag(nameof(Chest), GenerateGuid(1));
        public static readonly ITag Shoulders = new Tag(nameof(Shoulders), GenerateGuid(1));
        public static readonly ITag Hands = new Tag(nameof(Hands), GenerateGuid(1));
        public static readonly ITag Belt = new Tag(nameof(Belt), GenerateGuid(1));
        public static readonly ITag Legs = new Tag(nameof(Legs), GenerateGuid(1));
        public static readonly ITag Feet = new Tag(nameof(Feet), GenerateGuid(1));
        public static readonly ITag Offhand = new Tag(nameof(Offhand), GenerateGuid(1));
        public static readonly ITag MainHand = new Tag(nameof(MainHand), GenerateGuid(1));
        public static readonly ITag Neck = new Tag(nameof(Neck), GenerateGuid(1));
        public static readonly ITag LeftEar = new Tag(nameof(LeftEar), GenerateGuid(1));
        public static readonly ITag RightEar = new Tag(nameof(RightEar), GenerateGuid(1));

        static EquipmentTag()
        {
            TagList = new TagList( new [] 
            {
                Head, Chest, Shoulders, Hands, Belt, Legs, Feet, Offhand, MainHand, Neck, LeftEar, RightEar
            });
        }
        
        public static bool IsMember(ITag tag) => TagList.ContainsTag(tag);
        public static IEnumerable<ITag> GetMembers(ITagList tagList) => tagList.Where(IsMember);

        public static IReadOnlyList<ITag> Tags => TagList.Tags; 
        
        private static Guid GenerateGuid(int id)
        {
            
            var stringSuffix = id.ToString();
            var stringBody = new string('0', 12 - stringSuffix.Length);

            return new Guid($"00000001-0000-0000-0000-{stringBody}{stringSuffix}");
        }
    }
}