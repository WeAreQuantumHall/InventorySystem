namespace InventorySystem.Tags
{
    public static class EquipmentTag
    {
        public static readonly string Head = nameof(Head);
        public static readonly string Chest = nameof(Chest);
        public static readonly string Shoulders = nameof(Shoulders);
        public static readonly string Hands = nameof(Hands);
        public static readonly string Belt = nameof(Belt);
        public static readonly string Legs = nameof(Legs);
        public static readonly string Feet = nameof(Feet);
        public static readonly string OffHand = nameof(OffHand);
        public static readonly string MainHand = nameof(MainHand);
        public static readonly string Neck = nameof(Neck);
        public static readonly string LeftEar = nameof(LeftEar);
        public static readonly string RightEar = nameof(RightEar);

        public static bool IsMember(string tag) => TagUtils.EquipmentTags.ContainsTag(tag);
    }
}