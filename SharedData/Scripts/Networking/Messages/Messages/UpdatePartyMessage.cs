namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdatePartyMessage
    {
        public enum UpdateType : byte
        {
            Create,
            ChangeLeader,
            Setting,
            Terminate,
            Member,
        }
        public UpdateType type;
        public int id;
        public bool shareExp;
        public bool shareItem;
        public string characterId;
    }
}
