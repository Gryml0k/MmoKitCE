namespace MultiplayerARPG
{
    public enum SkillUsageType : byte
    {
        Skill,
        GuildSkill,
        UsableItem,
    }

    [System.Serializable]
    public partial struct CharacterSkillUsage
    {
        public static readonly CharacterSkillUsage Empty = new CharacterSkillUsage();
        public SkillUsageType type;
        public int dataId;
        public float coolDownRemainsDuration;

        public CharacterSkillUsage Clone()
        {
            return new CharacterSkillUsage()
            {
                type = type,
                dataId = dataId,
                coolDownRemainsDuration = coolDownRemainsDuration,
            };
        }

        public static CharacterSkillUsage Create(SkillUsageType type, int dataId)
        {
            return new CharacterSkillUsage()
            {
                type = type,
                dataId = dataId,
                coolDownRemainsDuration = 0f,
            };
        }
    }
}
