namespace MultiplayerARPG
{
    public partial class EditorMenuConsts
    {
        #region Build Settings
        public const string BUILD_SETUP_OFFLINE_LAN_MENU = "Tools/MmoKitCE/Build/Target/Setup For Offline-Lan Build";
        public const int BUILD_SETUP_OFFLINE_LAN_ORDER = -1000;
        public const string BUILD_SETUP_MMO_MENU = "Tools/MmoKitCE/Build/Target/Setup For MMO Build";
        public const int BUILD_SETUP_MMO_ORDER = -1000;
        public const string BUILD_SETUP_MMO_SERVER_INCLUDE_MENU = "Tools/MmoKitCE/Build/Target/Setup For MMO with Server Codes Build";
        public const int BUILD_SETUP_MMO_SERVER_INCLUDE_ORDER = -1000;
        public const string BUILD_SETUP_DISABLE_ADDRESSABLES_MENU = "Tools/MmoKitCE/Build//Addressables/Disable Addressables";
        public const int BUILD_SETUP_DISABLE_ADDRESSABLES_ORDER = 8;
        public const string BUILD_SETUP_ENABLE_ADDRESSABLES_MENU = "Tools/MmoKitCE/Build//Addressables/Enable Addressables";
        public const int BUILD_SETUP_ENABLE_ADDRESSABLES_ORDER = 9;
        public const string BUILD_SETUP_EXCLUDE_PREFAB_REFS_MENU = "Tools/MmoKitCE/Build//Addressables/Exclude Prefab Refs";
        public const int BUILD_SETUP_EXCLUDE_PREFAB_REFS_ORDER = 10;
        public const string BUILD_SETUP_INCLUDE_PREFAB_REFS_MENU = "Tools/MmoKitCE/Build//Addressables/Include Prefab Refs";
        public const int BUILD_SETUP_INCLUDE_PREFAB_REFS_ORDER = 11;
        public const string PREPARE_ADDRESSABLE_ASSETS_MENU = "Tools/MmoKitCE/Build//Addressables/Prepare Addressable Assets";
        public const int PREPARE_ADDRESSABLE_ASSETS_ORDER = 12;
        public const string BAKE_SERVER_SCENE_MENU = "Tools/MmoKitCE/Build//Scenes/Bake Server Scene";
        public const int BAKE_SERVER_SCENE_ORDER = 20;
        #endregion

        #region Database
        public const string GAME_DATABASE_MENU = "Tools/MmoKitCE/Develop/Game Database";
        public const int GAME_DATABASE_ORDER = 100;
        #endregion

        #region Entity (3D)
        public const string CHARACTER_ENTITY_CREATOR_MENU = "Tools/MmoKitCE/Develop/Character Entity Creator (3D)";
        public const int CHARACTER_ENTITY_CREATOR_ORDER = 200;

        public const string NPC_ENTITY_CREATOR_MENU = "Tools/MmoKitCE/Develop/Npc Entity Creator (3D)";
        public const int NPC_ENTITY_CREATOR_ORDER = 201;

        public const string VEHICLE_ENTITY_CREATOR_MENU = "Tools/MmoKitCE/Develop/Vehicle Entity Creator (3D)";
        public const int VEHICLE_ENTITY_CREATOR_ORDER = 202;
        #endregion

        #region Player Character Save
        public const string DELETE_ALL_PLAYER_CHARACTER_SAVES_MENU = "Tools/MmoKitCE/Develop/Delete All Player Character Saves";
        public const int DELETE_ALL_PLAYER_CHARACTER_SAVES_ORDER = 300;
        #endregion

        #region PlayerPrefs
        public const string DELETE_ALL_PLAYER_PREFS_MENU = "Tools/MmoKitCE/Utils/Delete PlayerPrefs";
        public const int DELETE_ALL_PLAYER_PREFS_ORDER = 0;
        #endregion

        #region Other
        public const string ITEM_CREATOR_MENU = "Tools/MmoKitCE/Develop/Item Creator";
        public const int ITEM_CREATOR_ORDER = 600;

        public const string MINIMAP_CREATOR_MENU = "Tools/MmoKitCE/Develop/Minimap Creator";
        public const int MINIMAP_CREATOR_ORDER = 601;
        #endregion
    }
}
