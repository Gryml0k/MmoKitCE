namespace MultiplayerARPG
{
    [System.Serializable]
    public struct GuildListEntry
    {
        public int Id { get; set; }
        public string GuildName { get; set; }
        public int Level { get; set; }
        public GuildListFieldOptions FieldOptions { get; set; }
        public string GuildMessage { get; set; }
        public string GuildMessage2 { get; set; }
        public int Score { get; set; }
        public string Options { get; set; }
        public bool AutoAcceptRequests { get; set; }
        public int Rank { get; set; }
        public int CurrentMembers { get; set; }
        public int MaxMembers { get; set; }
    }
}
