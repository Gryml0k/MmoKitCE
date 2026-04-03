namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct SocialCharacterData
    {
        public string id;
        public string userId;
        public string characterName;
        public int dataId;
        public int level;
        public int factionId;
        public int partyId;
        public int guildId;
        public byte guildRole;
        public int currentHp;
        public int maxHp;
        public int currentMp;
        public int maxMp;
        public int iconDataId;
        public int frameDataId;
        public int backgroundDataId;
        public int titleDataId;

        public static SocialCharacterData Create(IPlayerCharacterData character)
        {
            return new SocialCharacterData()
            {
                id = character.Id,
                characterName = character.CharacterName,
                dataId = character.DataId,
                level = character.Level,
                factionId = character.FactionId,
                partyId = character.PartyId,
                guildId = character.GuildId,
                guildRole = character.GuildRole,
                currentHp = character.CurrentHp,
                currentMp = character.CurrentMp,
                iconDataId = character.IconDataId,
                frameDataId = character.FrameDataId,
                backgroundDataId = character.BackgroundDataId,
                titleDataId = character.TitleDataId,
            };
        }
    }
}
