namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct MailListEntry
    {
        public string Id { get; set; }
        public string SenderName { get; set; }
        public string Title { get; set; }
        public bool IsRead { get; set; }
        public bool IsClaim { get; set; }
        public long SentTimestamp { get; set; }
    }
}
