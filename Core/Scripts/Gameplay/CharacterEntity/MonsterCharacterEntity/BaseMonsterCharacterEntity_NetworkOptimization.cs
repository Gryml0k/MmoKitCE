using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BaseMonsterCharacterEntity
    {
        /// <summary>
        /// When set by <see cref="MonsterSpawnArea"/> (including <see cref="MonsterSwarmArea"/>) before <see cref="LiteNetLibManager.LiteNetLibAssets.NetworkSpawn"/>,
        /// low-value character sync fields are not replicated and transform send thresholds are relaxed slightly.
        /// </summary>
        [System.NonSerialized]
        public bool bandwidthOptimizeFromSpawnArea;

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            if (!bandwidthOptimizeFromSpawnArea || SummonType != SummonType.None)
                return;

            ApplyLeanMonsterBandwidthSync();
        }

        private void ApplyLeanMonsterBandwidthSync()
        {
            exp.doNotSync = true;
            currentStamina.doNotSync = true;
            currentFood.doNotSync = true;
            currentWater.doNotSync = true;
        }
    }
}
