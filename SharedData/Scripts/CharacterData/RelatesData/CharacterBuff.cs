namespace MultiplayerARPG
{
    public enum BuffType : byte
    {
        SkillBuff,
        SkillDebuff,
        PotionBuff,
        GuildSkillBuff,
        StatusEffect,
    }

    [System.Serializable]
    public partial struct CharacterBuff
    {
        public static readonly CharacterBuff Empty = new CharacterBuff();
        public string id;
        public BuffType type;
        public int dataId;
        public int level;
        public float buffRemainsDuration;

        public CharacterBuff Clone(bool generateNewId = false)
        {
            return new CharacterBuff()
            {
                id = generateNewId || string.IsNullOrWhiteSpace(id) ? GenericUtils.GetUniqueId() : id,
                type = type,
                dataId = dataId,
                level = level,
                buffRemainsDuration = buffRemainsDuration,
            };
        }

        public static CharacterBuff Create(BuffType type, int dataId, int level = 1)
        {
            return new CharacterBuff()
            {
                id = GenericUtils.GetUniqueId(),
                type = type,
                dataId = dataId,
                level = level,
                buffRemainsDuration = 0f,
            };
        }
    }
}
