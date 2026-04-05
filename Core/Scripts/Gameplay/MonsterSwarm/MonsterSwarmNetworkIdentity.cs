using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System;
using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// Single networked identity representing many field monsters with a packed snapshot instead of one identity per mob.
    /// Server authoritative; clients render via <see cref="MonsterSwarmClientVisuals"/>.
    /// Damage entry point: <see cref="RequestHitFromClient"/> (validated on server by distance + RPC sender).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LiteNetLibIdentity))]
    public class MonsterSwarmNetworkIdentity : LiteNetLibBehaviour
    {
        public const int MaxSlots = 32;

        [Tooltip("How often to multicast swarm state to subscribed clients.")]
        public float snapshotInterval = 0.15f;

        [Tooltip("Max distance from player to slot world position for a hit to be accepted.")]
        public float hitMaxRange = 5f;

        [SerializeField]
        private MonsterSwarmClientVisuals clientVisuals;

        private SwarmSlotRuntime[] _slots = new SwarmSlotRuntime[MaxSlots];
        private MonsterCharacter _monsterData;
        private float _syncTimer;

        private struct SwarmSlotRuntime
        {
            public bool Active;
            public int DataId;
            public int Level;
            public int CurHp;
            public int MaxHp;
            public Vector3 Position;
        }

        private void Awake()
        {
            if (clientVisuals == null)
                clientVisuals = GetComponent<MonsterSwarmClientVisuals>();
        }

        private void Update()
        {
            if (!IsServer)
                return;

            ServerTickMovement(Time.deltaTime);
            _syncTimer += Time.deltaTime;
            if (_syncTimer < snapshotInterval)
                return;
            _syncTimer = 0f;
            BroadcastSnapshot();
        }

        /// <summary>
        /// Fills swarm slots from a spawn area and monster prefab (same stats for every slot). Server only.
        /// </summary>
        public void ServerPopulateFromSpawnArea(MonsterSpawnArea area, BaseMonsterCharacterEntity prefab, int level, int count)
        {
            if (!IsServer || area == null || prefab == null)
                return;

            _monsterData = prefab.CharacterDatabase;
            if (_monsterData == null)
            {
                Logging.LogError(ToString(), "Swarm populate: monster has no CharacterDatabase.");
                return;
            }

            count = Mathf.Clamp(count, 1, MaxSlots);
            int maxHp = SampleMaxHp(prefab, level);

            for (int i = 0; i < MaxSlots; i++)
                _slots[i] = default;

            int placed = 0;
            for (int i = 0; i < count; i++)
            {
                if (!area.GetRandomPosition(out Vector3 pos))
                    continue;

                _slots[placed] = new SwarmSlotRuntime
                {
                    Active = true,
                    DataId = _monsterData.DataId,
                    Level = level,
                    CurHp = maxHp,
                    MaxHp = maxHp,
                    Position = pos,
                };
                placed++;
            }

            BroadcastSnapshot();
        }

        private static int SampleMaxHp(BaseMonsterCharacterEntity prefab, int level)
        {
            GameObject temp = UnityEngine.Object.Instantiate(prefab.gameObject);
            temp.SetActive(false);
            var probe = temp.GetComponent<BaseMonsterCharacterEntity>();
            probe.Level = level;
            probe.InitStats();
            int hp = probe.MaxHp;
            UnityEngine.Object.Destroy(temp);
            return hp;
        }

        private void ServerTickMovement(float deltaTime)
        {
            float t = Time.time;
            for (int i = 0; i < MaxSlots; i++)
            {
                if (!_slots[i].Active)
                    continue;
                Vector3 p = _slots[i].Position;
                p.x += Mathf.Sin(t + i * 0.73f) * 0.2f * deltaTime;
                p.z += Mathf.Cos(t + i * 0.41f) * 0.2f * deltaTime;
                _slots[i].Position = p;
            }
        }

        private void BroadcastSnapshot()
        {
            if (!IsServer)
                return;

            NetDataWriter writer = new NetDataWriter();
            byte activeCount = 0;
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i].Active)
                    activeCount++;
            }

            writer.Put(activeCount);
            for (int i = 0; i < MaxSlots; i++)
            {
                if (!_slots[i].Active)
                    continue;
                writer.Put((byte)i);
                writer.Put(_slots[i].DataId);
                writer.Put(_slots[i].CurHp);
                writer.Put(_slots[i].MaxHp);
                Vector3 p = _slots[i].Position;
                writer.Put(p.x);
                writer.Put(p.y);
                writer.Put(p.z);
            }

            string b64 = Convert.ToBase64String(writer.Data, 0, writer.Length);
            RPC(nameof(RpcSwarmSnapshotB64), Identity.DefaultRpcChannelId, DeliveryMethod.Unreliable, RPCReceivers.All, b64);
        }

        [AllRpc]
        protected void RpcSwarmSnapshotB64(string payload)
        {
            if (IsServer)
                return;
            try
            {
                byte[] raw = Convert.FromBase64String(payload);
                NetDataReader reader = new NetDataReader();
                reader.SetSource(raw, 0, raw.Length);
                byte n = reader.GetByte();
                if (clientVisuals != null)
                    clientVisuals.BeginSnapshot();

                for (int c = 0; c < n; c++)
                {
                    byte slot = reader.GetByte();
                    int dataId = reader.GetInt();
                    int curHp = reader.GetInt();
                    int maxHp = reader.GetInt();
                    Vector3 pos = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                    if (clientVisuals != null)
                        clientVisuals.ApplySlot(slot, dataId, curHp, maxHp, pos);
                }

                if (clientVisuals != null)
                    clientVisuals.EndSnapshot();
            }
            catch (Exception ex)
            {
                Logging.LogWarning(ToString(), "Swarm snapshot parse failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Client calls this (e.g. from a slot collider) to request damage; server validates range and RPC sender.
        /// </summary>
        public void RequestHitFromClient(int slotIndex, int damageAmount)
        {
            if (!IsClient)
                return;
            damageAmount = Mathf.Clamp(damageAmount, 1, 100000);
            RPC(nameof(CmdHitSwarmSlot), Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, RPCReceivers.Server, slotIndex, damageAmount);
        }

        [ServerRpc(canCallByEveryone = true)]
        private void CmdHitSwarmSlot(int slotIndex, int damageAmount)
        {
            if (!IsServer)
                return;

            long conn = BaseGameNetworkManager.IncomingClientRpcConnectionId;
            if (conn < 0 || !Manager.TryGetPlayer(conn, out LiteNetLibPlayer player))
                return;

            slotIndex = Mathf.Clamp(slotIndex, 0, MaxSlots - 1);
            if (!_slots[slotIndex].Active)
                return;

            BasePlayerCharacterEntity playerEntity = null;
            foreach (LiteNetLibIdentity idn in player.GetSpawnedObjects())
            {
                playerEntity = idn.GetComponent<BasePlayerCharacterEntity>();
                if (playerEntity != null)
                    break;
            }

            if (playerEntity == null || playerEntity.IsDead())
                return;

            float dist = Vector3.Distance(playerEntity.EntityTransform.position, _slots[slotIndex].Position);
            if (dist > hitMaxRange)
                return;

            damageAmount = Mathf.Clamp(damageAmount, 1, 100000);
            SwarmSlotRuntime s = _slots[slotIndex];
            s.CurHp -= damageAmount;
            if (s.CurHp <= 0)
            {
                int lvl = s.Level;
                s.Active = false;
                _slots[slotIndex] = s;
                GiveKillRewards(playerEntity, lvl);
            }
            else
            {
                _slots[slotIndex] = s;
            }

            BroadcastSnapshot();
        }

        private void GiveKillRewards(BasePlayerCharacterEntity killer, int level)
        {
            if (_monsterData == null || killer == null)
                return;

            int exp = _monsterData.RandomExp(level);
            killer.RewardExp(exp, 1f, RewardGivenType.KillMonster, level, level);
            int gold = _monsterData.RandomGold(level);
            killer.RewardGold(gold, 1f, RewardGivenType.KillMonster, level, level);
        }
    }
}
