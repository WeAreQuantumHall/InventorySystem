using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Abstractions.Tags;

namespace InventorySystem.Tags
{
    public static class EquipmentTag
    {
        static EquipmentTag()
        {
            TagList = new TagList( new [] 
            {
                Head, Chest, Shoulders, Hands, Belt, Legs, Feet, Offhand, MainHand, Neck, LeftEar, RightEar
            });
        }
        
        private static readonly ITagList TagList;

        public static readonly ITag Head = new Tag(nameof(Head), GenerateGuid(1));
        public static readonly ITag Chest = new Tag(nameof(Chest), GenerateGuid(2));
        public static readonly ITag Shoulders = new Tag(nameof(Shoulders), GenerateGuid(3));
        public static readonly ITag Hands = new Tag(nameof(Hands), GenerateGuid(4));
        public static readonly ITag Belt = new Tag(nameof(Belt), GenerateGuid(5));
        public static readonly ITag Legs = new Tag(nameof(Legs), GenerateGuid(6));
        public static readonly ITag Feet = new Tag(nameof(Feet), GenerateGuid(7));
        public static readonly ITag Offhand = new Tag(nameof(Offhand), GenerateGuid(8));
        public static readonly ITag MainHand = new Tag(nameof(MainHand), GenerateGuid(9));
        public static readonly ITag Neck = new Tag(nameof(Neck), GenerateGuid(10));
        public static readonly ITag LeftEar = new Tag(nameof(LeftEar), GenerateGuid(11));
        public static readonly ITag RightEar = new Tag(nameof(RightEar), GenerateGuid(12));

        public static IReadOnlyList<ITag> Tags => TagList.Tags; 
        
        public static bool IsMember(ITag tag) => TagList.ContainsTag(tag);
        public static IEnumerable<ITag> GetMembers(ITagList tagList) => tagList.Tags.Where(IsMember);
        
        private static Guid GenerateGuid(int id)
        {
            var stringSuffix = id.ToString();
            var stringBody = new string('0', 12 - stringSuffix.Length);

            return new Guid($"00000001-0000-0000-0000-{stringBody}{stringSuffix}");
        }
    }
}