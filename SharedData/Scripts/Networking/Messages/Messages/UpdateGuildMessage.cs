namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateGuildMessage
    {
        public enum UpdateType : byte
        {
            Create,
            ChangeLeader,
            SetGuildMessage,
            SetGuildMessage2,
            SetGuildRole,
            SetGuildMemberRole,
            SetSkillLevel,
            LevelExpSkillPoint,
            Terminate,
            SetGold,
            SetScore,
            SetOptions,
            SetAutoAcceptRequests,
            SetRank,
            UpdateStorage,
            Member,
        }
        public UpdateType type;
        public int id;
        public string guildName;
        public string guildMessage;
        public string characterId;
        public byte guildRole;
        public GuildRoleData guildRoleData;
        public int level;
        public int exp;
        public int skillPoint;
        public int gold;
        public int score;
        public string options;
        public bool autoAcceptRequests;
        public int rank;
        public int dataId;
    }
}
