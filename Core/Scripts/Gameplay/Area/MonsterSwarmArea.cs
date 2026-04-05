using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// Like <see cref="MonsterSpawnArea"/>, but spawns a single <see cref="MonsterSwarmNetworkIdentity"/> that represents
    /// many mobs in one network object (see bandwidth plan Phase 2). Use <see cref="MonsterSpawnArea"/> for classic one-identity-per-monster spawning.
    /// Requires the main <c>prefab</c> only (no entries in <c>spawningPrefabs</c>).
    /// </summary>
    public class MonsterSwarmArea : MonsterSpawnArea
    {
        [Header("Monster swarm (blob)")]
        [Tooltip("Prefab with LiteNetLibIdentity + MonsterSwarmNetworkIdentity (+ MonsterSwarmClientVisuals on clients).")]
        public MonsterSwarmNetworkIdentity swarmNetworkPrefab;

        private MonsterSwarmNetworkIdentity _spawnedSwarmInstance;

        public override void RegisterPrefabs()
        {
            base.RegisterPrefabs();
            if (swarmNetworkPrefab != null && BaseGameNetworkManager.Singleton != null)
                BaseGameNetworkManager.Singleton.Assets.RegisterPrefab(swarmNetworkPrefab.Identity);
        }

        public override void SpawnAll()
        {
#if !DISABLE_ADDRESSABLES
            if (addressablePrefab.IsDataValid() && prefab == null)
            {
                Logging.LogWarning(ToString(), "MonsterSwarmArea does not support addressable-only monster spawners; using normal spawn.");
                base.SpawnAll();
                return;
            }
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            if (swarmNetworkPrefab != null &&
                prefab != null &&
                spawningPrefabs.Count == 0 &&
                BaseGameNetworkManager.Singleton != null &&
                BaseGameNetworkManager.Singleton.IsServer)
            {
#if !DISABLE_ADDRESSABLES
                if (addressablePrefab.IsDataValid())
                {
                    base.SpawnAll();
                    return;
                }
#endif
                EnsureSwarmAndPopulate();
                return;
            }
#endif
            if (swarmNetworkPrefab == null)
                Logging.LogWarning(ToString(), "MonsterSwarmArea: assign swarmNetworkPrefab to use blob spawning; falling back to classic spawn.");
            base.SpawnAll();
        }

        private void EnsureSwarmAndPopulate()
        {
            int amount = GetRandomedSpawnAmount();
            int level = GetRandomedSpawnLevel();
            if (_spawnedSwarmInstance == null || !_spawnedSwarmInstance.IsSpawned)
            {
                LiteNetLibIdentity swarmObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    swarmNetworkPrefab.Identity.HashAssetId,
                    transform.position,
                    transform.rotation);
                if (swarmObj == null)
                {
                    Logging.LogError(ToString(), "Monster swarm: failed to instantiate swarmNetworkPrefab.");
                    return;
                }
                swarmObj.SubChannelId = subChannelId;
                if (monsterSubscriberVisibleRange > 0f)
                    swarmObj.VisibleRange = monsterSubscriberVisibleRange;
                _spawnedSwarmInstance = swarmObj.GetComponent<MonsterSwarmNetworkIdentity>();
                if (_spawnedSwarmInstance == null)
                {
                    BaseGameNetworkManager.Singleton.Assets.DestroyObjectInstance(swarmObj);
                    Logging.LogError(ToString(), "Monster swarm: swarmNetworkPrefab must include MonsterSwarmNetworkIdentity.");
                    return;
                }
                BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(swarmObj);
                _subscribeHandler.AddEntity(_spawnedSwarmInstance, null);
            }

            _spawnedSwarmInstance.ServerPopulateFromSpawnArea(this, prefab, level, amount);
        }
    }
}
