using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateSocialMembersMessage
    {
        public int id;
        public List<SocialCharacterData> members;
    }
}
