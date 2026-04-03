namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct ChatMessage
    {
        public ChatChannel channel;
        public string message;
        public string senderUserId;
        public string senderId;
        public string senderName;
        public string receiverUserId;
        public string receiverId;
        public string receiverName;
        public int guildId;
        public string guildName;
        public int channelId;
        public bool sendByServer;
        public long timestamp;
    }
}
