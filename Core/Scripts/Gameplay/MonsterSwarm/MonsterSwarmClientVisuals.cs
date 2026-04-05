using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// Client-side pooled transforms for <see cref="MonsterSwarmNetworkIdentity"/> snapshots.
    /// Assign a simple prefab with a collider; optional <see cref="MonsterSwarmSlotClickResponder"/> is added for testing hits.
    /// </summary>
    [DisallowMultipleComponent]
    public class MonsterSwarmClientVisuals : MonoBehaviour
    {
        [Tooltip("Simple representation (capsule/cube) with a collider for click tests.")]
        public GameObject visualPrefab;

        [Tooltip("Parent for pooled instances; defaults to this transform.")]
        public Transform visualsRoot;

        private MonsterSwarmNetworkIdentity _swarm;
        private Transform[] _pool = new Transform[MonsterSwarmNetworkIdentity.MaxSlots];

        private void Awake()
        {
            _swarm = GetComponent<MonsterSwarmNetworkIdentity>();
            if (visualsRoot == null)
                visualsRoot = transform;
        }

        public void BeginSnapshot()
        {
            for (int i = 0; i < MonsterSwarmNetworkIdentity.MaxSlots; i++)
            {
                if (_pool[i] != null)
                    _pool[i].gameObject.SetActive(false);
            }
        }

        public void ApplySlot(byte slot, int dataId, int curHp, int maxHp, Vector3 position)
        {
            if (slot >= MonsterSwarmNetworkIdentity.MaxSlots)
                return;

            Transform t = _pool[slot];
            if (t == null && visualPrefab != null)
            {
                GameObject inst = Instantiate(visualPrefab, visualsRoot);
                t = inst.transform;
                _pool[slot] = t;
                var click = inst.GetComponent<MonsterSwarmSlotClickResponder>();
                if (click == null)
                    click = inst.AddComponent<MonsterSwarmSlotClickResponder>();
                click.Setup(_swarm, slot);
            }

            if (t == null)
                return;

            t.gameObject.SetActive(true);
            t.position = position;
            t.name = $"SwarmSlot_{slot}_hp{curHp}";
        }

        public void EndSnapshot()
        {
            // Slots not mentioned stay inactive from BeginSnapshot.
        }
    }

    /// <summary>
    /// Dev-oriented click hook: sends a fixed damage request to the swarm authority.
    /// </summary>
    [DisallowMultipleComponent]
    public class MonsterSwarmSlotClickResponder : MonoBehaviour
    {
        [SerializeField]
        private int testDamage = 25;

        private MonsterSwarmNetworkIdentity _swarm;
        private int _slot;

        public void Setup(MonsterSwarmNetworkIdentity swarm, int slot)
        {
            _swarm = swarm;
            _slot = slot;
        }

        private void OnMouseDown()
        {
            if (_swarm != null)
                _swarm.RequestHitFromClient(_slot, testDamage);
        }
    }
}
