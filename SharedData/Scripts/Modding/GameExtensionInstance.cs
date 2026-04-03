using Insthync.DevExtension;

namespace MultiplayerARPG
{
    public static partial class GameExtensionInstance
    {
        public static CharacterItemCloneDelegate onCharacterItemClone;

        static GameExtensionInstance()
        {
            DevExtUtils.InvokeStaticDevExtMethods(typeof(GameExtensionInstance), "Init");
        }
    }
}
