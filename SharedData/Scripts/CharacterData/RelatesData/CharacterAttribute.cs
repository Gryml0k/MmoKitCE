namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterAttribute
    {
        public static readonly CharacterAttribute Empty = new CharacterAttribute();
        public int dataId;
        public int amount;

        public CharacterAttribute Clone()
        {
            return new CharacterAttribute()
            {
                dataId = dataId,
                amount = amount,
            };
        }

        public static CharacterAttribute Create(int dataId, int amount = 0)
        {
            return new CharacterAttribute()
            {
                dataId = dataId,
                amount = amount,
            };
        }
    }
}
