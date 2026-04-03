namespace MultiplayerARPG
{
    public enum MountType : byte
    {
        None,
        Skill,
        MountItem,
        Buff,
        Custom = 254,
    }

    [System.Serializable]
    public partial struct CharacterMount
    {
        public static readonly CharacterMount Empty = new CharacterMount();
        public MountType type;
        public string sourceId;
        public float mountRemainsDuration;
        public int level;
        public int currentHp;

        public CharacterMount Clone()
        {
            CharacterMount result = new CharacterMount()
            {
                type = type,
                sourceId = sourceId,
                mountRemainsDuration = mountRemainsDuration,
                level = level,
                currentHp = currentHp,
            };
            return result;
        }
    }
}
