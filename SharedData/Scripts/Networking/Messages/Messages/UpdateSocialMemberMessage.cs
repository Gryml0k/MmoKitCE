namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateSocialMemberMessage
    {
        public enum UpdateType : byte
        {
            Add,
            Update,
            Remove,
            Clear,
        }
        public UpdateType type;
        public int socialId;
        public SocialCharacterData character;
    }
}
