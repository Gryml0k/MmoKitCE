using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterItem
    {
        public static readonly CharacterItem Empty = new CharacterItem();
        public string id;
        public int dataId;
        public int level;
        public int amount;
        public byte equipSlotIndex;
        public float durability;
        public int exp;
        public float lockRemainsDuration;
        public long expireTime;
        public int randomSeed;
        public int ammoDataId;
        public int ammo;
        public List<int> sockets;
        public byte version;

        public List<int> ReadSockets(string socketsString, char separator = ';')
        {
            sockets = socketsString.ReadCharacterItemSockets(separator);
            return sockets;
        }

        public string WriteSockets(char separator = ';')
        {
            return sockets.WriteCharacterItemSockets(separator);
        }

        public CharacterItem Clone(bool generateNewId = false)
        {
            List<int> sockets = this.sockets == null ? new List<int>() : new List<int>(this.sockets);
            CharacterItem destination = new CharacterItem()
            {
                id = generateNewId || string.IsNullOrWhiteSpace(id) ? GenericUtils.GetUniqueId() : id,
                dataId = dataId,
                level = level,
                amount = amount,
                equipSlotIndex = equipSlotIndex,
                durability = durability,
                exp = exp,
                lockRemainsDuration = lockRemainsDuration,
                expireTime = expireTime,
                randomSeed = randomSeed,
                ammoDataId = ammoDataId,
                ammo = ammo,
                sockets = sockets,
                version = version,
            };
            if (GameExtensionInstance.onCharacterItemClone == null)
                return destination;
            return GameExtensionInstance.onCharacterItemClone(ref this, destination);
        }
    }
}
