namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct GuildRoleData
    {
        public string roleName;
        public bool canInvite;
        public bool canKick;
        public bool canUseStorage;
        public byte shareExpPercentage;
    }
}
