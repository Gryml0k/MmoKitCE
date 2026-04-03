using Cysharp.Text;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UnlockableContent
    {
        public UnlockableContentType type;
        public int dataId;
        public int progression;
        public bool unlocked;

        public string GetId()
        {
            return ZString.Concat((byte)type, dataId);
        }
    }
}
