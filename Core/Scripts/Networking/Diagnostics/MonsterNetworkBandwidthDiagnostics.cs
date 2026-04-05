using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// Optional dev aid: periodic server-side summary of spawned monsters and where bandwidth usually goes.
    /// Add to the same GameObject as <see cref="BaseGameNetworkManager"/> (or any active object) and enable logging.
    /// </summary>
    [DisallowMultipleComponent]
    public class MonsterNetworkBandwidthDiagnostics : MonoBehaviour
    {
        [Tooltip("Seconds between log lines when enabled.")]
        public float reportIntervalSeconds = 10f;

        [Tooltip("When true, logs a short report on the server while playing.")]
        public bool logReportsOnServer = true;

        private float _timer;

        private void Update()
        {
            if (!logReportsOnServer)
                return;
            var mgr = BaseGameNetworkManager.Singleton;
            if (mgr == null || !mgr.IsServer)
                return;

            _timer += Time.unscaledDeltaTime;
            if (_timer < reportIntervalSeconds)
                return;
            _timer = 0f;

            int monsters = 0;
            int withTransform = 0;
            foreach (LiteNetLibIdentity identity in mgr.Assets.GetSpawnedObjects())
            {
                if (identity == null)
                    continue;
                var m = identity.GetComponent<BaseMonsterCharacterEntity>();
                if (m == null)
                    continue;
                monsters++;
                if (identity.GetComponent<LiteNetLibTransform>() != null)
                    withTransform++;
            }

            Logging.Log(nameof(MonsterNetworkBandwidthDiagnostics),
                $"Monster bandwidth baseline: spawned_monsters={monsters}, with_LiteNetLibTransform={withTransform}. " +
                "Typical per-mob cost: (1) LiteNetLibTransform ServerSyncTransform RPC each logic tick while moving, " +
                "(2) BaseCharacterEntity SyncFields + SyncLists on spawn and when stats change, " +
                "(3) interest manager subscription radius. Use MonsterSpawnArea.optimizeSpawnedMonsterBandwidth / monsterSubscriberVisibleRange to trim; use MonsterSwarmArea for blob swarms.");
        }
    }
}
