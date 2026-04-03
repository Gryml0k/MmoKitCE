namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterSkill
    {
        public static readonly CharacterSkill Empty = new CharacterSkill();
        public int dataId;
        public int level;

        public CharacterSkill Clone()
        {
            return new CharacterSkill()
            {
                dataId = dataId,
                level = level,
            };
        }

        public static CharacterSkill Create(int dataId, int level = 1)
        {
            return new CharacterSkill()
            {
                dataId = dataId,
                level = level,
            };
        }
    }
}
