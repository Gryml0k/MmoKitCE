namespace MultiplayerARPG
{
    [System.Serializable]
    public struct CharacterDataInt32
    {
        public int hashedKey;
        public int value;

        public CharacterDataInt32 Clone()
        {
            return new CharacterDataInt32()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }

        public static CharacterDataInt32 Create(int hashedKey, int value = 0)
        {
            return new CharacterDataInt32()
            {
                hashedKey = hashedKey,
                value = value,
            };
        }
    }
}