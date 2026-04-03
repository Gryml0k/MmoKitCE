using Insthync.DevExtension;

namespace MultiplayerARPG
{
    public static partial class PlayerCharacterDataExtensions
    {
        public static System.Type ClassType { get; private set; }

        static PlayerCharacterDataExtensions()
        {
            ClassType = typeof(PlayerCharacterDataExtensions);
        }

        public static T CloneTo<T>(this IPlayerCharacterData from, T to,
            bool withEquipWeapons = true,
            bool withAttributes = true,
            bool withSkills = true,
            bool withSkillUsages = true,
            bool withBuffs = true,
            bool withEquipItems = true,
            bool withNonEquipItems = true,
            bool withSummons = true,
            bool withHotkeys = true,
            bool withQuests = true,
            bool withCurrencies = true,
            bool withServerCustomData = true,
            bool withPrivateCustomData = true,
            bool withPublicCustomData = true,
            bool withCurrentLocationData = true) where T : IPlayerCharacterData
        {
            to.Id = from.Id;
            to.DataId = from.DataId;
            to.EntityId = from.EntityId;
            to.UserId = from.UserId;
            to.FactionId = from.FactionId;
            to.CharacterName = from.CharacterName;
            to.Level = from.Level;
            to.Exp = from.Exp;
            to.CurrentHp = from.CurrentHp;
            to.CurrentMp = from.CurrentMp;
            to.CurrentStamina = from.CurrentStamina;
            to.CurrentFood = from.CurrentFood;
            to.CurrentWater = from.CurrentWater;
            to.StatPoint = from.StatPoint;
            to.SkillPoint = from.SkillPoint;
            to.Gold = from.Gold;
            to.UserGold = from.UserGold;
            to.UserCash = from.UserCash;
            to.PartyId = from.PartyId;
            to.GuildId = from.GuildId;
            to.GuildRole = from.GuildRole;
            to.EquipWeaponSet = from.EquipWeaponSet;
            if (withCurrentLocationData)
            {
                to.CurrentChannel = from.CurrentChannel;
                to.CurrentMapName = from.CurrentMapName;
                to.CurrentPosition = from.CurrentPosition;
                to.CurrentRotation = from.CurrentRotation;
                to.CurrentSafeArea = from.CurrentSafeArea;
            }
#if !DISABLE_DIFFER_MAP_RESPAWNING
            to.RespawnMapName = from.RespawnMapName;
            to.RespawnPosition = from.RespawnPosition;
#endif
            to.IconDataId = from.IconDataId;
            to.FrameDataId = from.FrameDataId;
            to.TitleDataId = from.TitleDataId;
            to.LastDeadTime = from.LastDeadTime;
            to.UnmuteTime = from.UnmuteTime;
            to.LastUpdate = from.LastUpdate;
#if !DISABLE_CLASSIC_PK
            to.IsPkOn = from.IsPkOn;
            to.LastPkOnTime = from.LastPkOnTime;
            to.PkPoint = from.PkPoint;
            to.ConsecutivePkKills = from.ConsecutivePkKills;
            to.HighestPkPoint = from.HighestPkPoint;
            to.HighestConsecutivePkKills = from.HighestConsecutivePkKills;
#endif
            to.Reputation = from.Reputation;
            if (withEquipWeapons)
                to.SelectableWeaponSets = from.SelectableWeaponSets.Clone();
            if (withAttributes)
                to.Attributes = from.Attributes.Clone();
            if (withSkills)
                to.Skills = from.Skills.Clone();
            if (withSkillUsages)
                to.SkillUsages = from.SkillUsages.Clone();
            if (withBuffs)
                to.Buffs = from.Buffs.Clone();
            if (withEquipItems)
                to.EquipItems = from.EquipItems.Clone();
            if (withNonEquipItems)
                to.NonEquipItems = from.NonEquipItems.Clone();
            if (withSummons)
                to.Summons = from.Summons.Clone();
            if (withHotkeys)
                to.Hotkeys = from.Hotkeys.Clone();
            if (withQuests)
                to.Quests = from.Quests.Clone();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (withCurrencies)
                to.Currencies = from.Currencies.Clone();
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            if (withServerCustomData)
            {
                to.ServerBools = from.ServerBools.Clone();
                to.ServerInts = from.ServerInts.Clone();
                to.ServerFloats = from.ServerFloats.Clone();
            }
            if (withPrivateCustomData)
            {
                to.PrivateBools = from.PrivateBools.Clone();
                to.PrivateInts = from.PrivateInts.Clone();
                to.PrivateFloats = from.PrivateFloats.Clone();
            }
            if (withPublicCustomData)
            {
                to.PublicBools = from.PublicBools.Clone();
                to.PublicInts = from.PublicInts.Clone();
                to.PublicFloats = from.PublicFloats.Clone();
            }
#endif
            to.GuildSkills = from.GuildSkills.Clone();
            to.Mount = from.Mount.Clone();
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "CloneTo", from, to);
            return to;
        }

        public static int IndexOfHotkey(this IPlayerCharacterData data, string hotkeyId)
        {
            return data.Hotkeys.IndexOf(hotkeyId);
        }

        public static int IndexOfQuest(this IPlayerCharacterData data, int dataId)
        {
            return data.Quests.IndexOf(dataId);
        }

        public static int IndexOfCurrency(this IPlayerCharacterData data, int dataId)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            return data.Currencies.IndexOf(dataId);
#else
            return -1;
#endif
        }

        public static int IndexOfServerBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfServerInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfServerFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static bool GetServerBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetServerBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetServerInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetServerInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetServerFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetServerFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerFloats.SetValue(hashedKey, value);
#endif
        }

        public static bool GetPrivateBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetPrivateBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetPrivateInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetPrivateInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetPrivateFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetPrivateFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateFloats.SetValue(hashedKey, value);
#endif
        }

        public static bool GetPublicBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetPublicBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetPublicInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetPublicInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetPublicFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetPublicFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicFloats.SetValue(hashedKey, value);
#endif
        }
    }
}
