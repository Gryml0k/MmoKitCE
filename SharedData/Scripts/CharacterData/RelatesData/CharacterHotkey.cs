namespace MultiplayerARPG
{
    public enum HotkeyType : byte
    {
        None,
        Skill,
        Item,
        GuildSkill,
    }

    [System.Serializable]
    public partial struct CharacterHotkey
    {
        public static readonly CharacterHotkey Empty = new CharacterHotkey();
        public string hotkeyId;
        public HotkeyType type;
        public string relateId;

        public CharacterHotkey Clone()
        {
            return new CharacterHotkey()
            {
                hotkeyId = hotkeyId,
                type = type,
                relateId = relateId,
            };
        }
    }
}
