// ce stability: #46

using Insthync.SpatialPartitioningSystems;
using LiteNetLibManager;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;

namespace MultiplayerARPG
{
    public class JobifiedGridSpatialPartitioningAOI : BaseInterestManager
    {
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("JobifiedGridSpatialPartitioningAOI - Update");
        protected static readonly ProfilerMarker s_CompleteProfilerMarker = new ProfilerMarker("JobifiedGridSpatialPartitioningAOI - Complete");

        public float cellSize = 64f;
        public int maxObjects = 10000;
        [Tooltip("Update every ? seconds")]
        public float updateInterval = 1.0f;
        public Vector3 bufferedCells = Vector3.one;
        public Color boundsGizmosColor = Color.green;

        private JobifiedGridSpatialPartitioningSystem _system;
        private float _updateCountDown;
        private Bounds _bounds;
        private Dictionary<uint, HashSet<uint>> _playerSubscribings = new Dictionary<uint, HashSet<uint>>();
        private readonly Queue<HashSet<uint>> _hashSetPool = new Queue<HashSet<uint>>();
        private readonly HashSet<uint> _fallbackSubscribings = new HashSet<uint>();
        private HashSet<uint> _alwaysVisibleObjects = new HashSet<uint>();
        private bool _didLogMissingSystemWarning;

        public bool IsSystemReady => _system != null;
        private Queue<NativeList<SpatialObject>> _queryQueue = new Queue<NativeList<SpatialObject>>();
        private Dictionary<uint, NativeList<SpatialObject>> _queryingPlayerSubscribings = new Dictionary<uint, NativeList<SpatialObject>>();
        private Dictionary<uint, NativeList<SpatialObject>> _queryingComponentSubscribings = new Dictionary<uint, NativeList<SpatialObject>>();
        private bool _isQuerying = false;

        public NativeList<SpatialObject> ReserveResultList()
        {
            if (_queryQueue.Count > 0)
            {
                return _queryQueue.Dequeue();
            }
            else
            {
                return new NativeList<SpatialObject>(1024, Allocator.Persistent);
            }
        }

        public void ReturnResultList(NativeList<SpatialObject> resultList)
        {
            resultList.Clear();
            _queryQueue.Enqueue(resultList);
        }

        public void DisposeResultLists()
        {
            foreach (var resultList in _queryingPlayerSubscribings)
            {
                if (resultList.Value.IsCreated)
                    resultList.Value.Dispose();
            }
            foreach (var resultList in _queryingComponentSubscribings)
            {
                if (resultList.Value.IsCreated)
                    resultList.Value.Dispose();
            }
            while (_queryQueue.Count > 0)
            {
                NativeList<SpatialObject> resultList = _queryQueue.Dequeue();
                if (resultList.IsCreated)
                    resultList.Dispose();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color color = Gizmos.color;
            Gizmos.color = boundsGizmosColor;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            Gizmos.color = color;
        }

        private void OnDestroy()
        {
            ReleaseSystem();
        }

        private void ReleaseSystem()
        {
            if (_system == null)
                return;
            _system.Dispose();
            _system = null;
            DisposeResultLists();
        }

        public override void Setup(LiteNetLibGameManager manager)
        {
            base.Setup(manager);
            manager.Assets.onLoadSceneFinish.RemoveListener(OnLoadSceneFinish);
            PrepareSystem();
            manager.Assets.onLoadSceneFinish.AddListener(OnLoadSceneFinish);
        }

        private void OnLoadSceneFinish(string sceneName, bool isAdditive, bool isOnline, float progress)
        {
            if (!IsServer || !isOnline)
            {
                ReleaseSystem();
                _didLogMissingSystemWarning = false;
                return;
            }
            // Full online scene load is already handled by OnServerOnlineSceneLoaded().
            // Keep this callback for additive scene changes that can alter AOI bounds.
            if (!isAdditive)
                return;
            PrepareSystem();
        }

        public void PrepareSystem()
        {
            if (!IsServer || !Manager.ServerSceneInfo.HasValue)
            {
                ReleaseSystem();
                _didLogMissingSystemWarning = false;
                return;
            }
            ReleaseSystem();
            var mapBounds = GenericUtils.GetComponentsFromAllLoadedScenes<AOIMapBounds>(true);
            if (mapBounds.Count > 0)
            {
                _bounds = mapBounds[0].GetBounds();
                for (int i = 0; i < mapBounds.Count; ++i)
                {
                    _bounds.Encapsulate(mapBounds[i].GetBounds());
                }
                _bounds.extents += bufferedCells * cellSize * 2;
                switch (GameInstance.Singleton.DimensionType)
                {
                    case DimensionType.Dimension3D:
                        _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, true, false);
                        break;
                    case DimensionType.Dimension2D:
                        _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, false, true);
                        break;
                }
            }
            else
            {
                switch (GameInstance.Singleton.DimensionType)
                {
                    case DimensionType.Dimension3D:
                        var collider3Ds = GenericUtils.GetComponentsFromAllLoadedScenes<Collider>(true);
                        if (collider3Ds.Count > 0)
                        {
                            _bounds = collider3Ds[0].bounds;
                            for (int i = 1; i < collider3Ds.Count; ++i)
                            {
                                _bounds.Encapsulate(collider3Ds[i].bounds);
                            }
                            _bounds.extents += bufferedCells * cellSize * 2;
                            _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, true, false);
                        }
                        break;
                    case DimensionType.Dimension2D:
                        var collider2Ds = GenericUtils.GetComponentsFromAllLoadedScenes<Collider2D>(true);
                        if (collider2Ds.Count > 0)
                        {
                            _bounds = collider2Ds[0].bounds;
                            for (int i = 1; i < collider2Ds.Count; ++i)
                            {
                                _bounds.Encapsulate(collider2Ds[i].bounds);
                            }
                            _bounds.extents += bufferedCells * cellSize * 2;
                            _system = new JobifiedGridSpatialPartitioningSystem(_bounds, cellSize, maxObjects, false, false, true);
                        }
                        break;
                }
            }

            if (_system == null && IsServer && Manager.ServerSceneInfo.HasValue)
            {
                if (!_didLogMissingSystemWarning)
                {
                    Debug.LogWarning("[AOI] PrepareSystem: No AOIMapBounds or scene colliders found. " +
                        "Grid AOI system could not initialize. Interest management will run in degraded " +
                        "fallback mode (O(N*M) range checks). Add an AOIMapBounds component to your map scene.");
                    _didLogMissingSystemWarning = true;
                }
            }
            else
            {
                _didLogMissingSystemWarning = false;
            }
        }

        public override void UpdateInterestManagementImmediate()
        {
            _updateCountDown = 0f;
            UpdateInterestManagement(0f);
        }

        public override void UpdateInterestManagement(float deltaTime)
        {
            if (_system == null)
            {
                FallbackUpdateInterestManagement(deltaTime);
                return;
            }

            _updateCountDown -= deltaTime;
            if (_updateCountDown > 0)
                return;
            _updateCountDown = updateInterval;

            foreach (KeyValuePair<uint, HashSet<uint>> kv in _playerSubscribings)
            {
                kv.Value.Clear();
                _hashSetPool.Enqueue(kv.Value);
            }
            _playerSubscribings.Clear();

            using (s_UpdateProfilerMarker.Auto())
            {
                _system.ClearObjects();
                var players = Manager.GetPlayers();
                while (players.MoveNext())
                {
                    LiteNetLibPlayer player = players.Current.Value;
                    if (!player.IsReady)
                    {
                        // Don't subscribe if player not ready
                        continue;
                    }
                    var playerObjs = player.GetSpawnedObjects();
                    while (playerObjs.MoveNext())
                    {
                        LiteNetLibIdentity playerObj = playerObjs.Current.Value;
                        _system.AddObjectToGrid(new SpatialObject()
                        {
                            objectId = playerObj.ObjectId,
                            position = playerObj.transform.position,
                        });
                    }
                }
                _system.UpdateGrid();

                _alwaysVisibleObjects.Clear();
                var spawnedObjects = Manager.Assets.GetSpawnedObjects();
                while (spawnedObjects.MoveNext())
                {
                    LiteNetLibIdentity spawnedObject = spawnedObjects.Current.Value;
                    if (spawnedObject == null)
                        continue;
                    if (spawnedObject.AlwaysVisible)
                    {
                        _alwaysVisibleObjects.Add(spawnedObject.ObjectId);
                        continue;
                    }
                    NativeList<SpatialObject> nativeQueryResult = ReserveResultList();
                    _queryingPlayerSubscribings[spawnedObject.ObjectId] = nativeQueryResult;
                    _system.QuerySphere(spawnedObject.transform.position, GetVisibleRange(spawnedObject), nativeQueryResult);
                }

                var components = SpatialObjectContainer.GetEnumerator();
                while (components.MoveNext())
                {
                    ISpatialObjectComponent component = components.Current.Value;
                    if (component == null)
                        continue;
                    component.ClearSubscribers();
                    if (!component.SpatialObjectEnabled)
                        continue;
                    NativeList<SpatialObject> nativeQueryResult = ReserveResultList();
                    _queryingComponentSubscribings[component.SpatialObjectId] = nativeQueryResult;
                    switch (component.SpatialObjectShape)
                    {
                        case SpatialObjectShape.Box:
                            _system.QueryBox(component.SpatialObjectPosition, component.SpatialObjectExtents, nativeQueryResult);
                            break;
                        default:
                            _system.QuerySphere(component.SpatialObjectPosition, component.SpatialObjectRadius, nativeQueryResult);
                            break;
                    }
                }

                _isQuerying = true;
            }
        }

        private void LateUpdate()
        {
            if (!_isQuerying)
                return;

            _isQuerying = false;

            using (s_CompleteProfilerMarker.Auto())
            {
                _system.Complete();

                NativeList<SpatialObject> queryResult;
                HashSet<uint> subscribings;
                LiteNetLibIdentity foundPlayerObject;
                foreach (var playerQueryKvp in _queryingPlayerSubscribings)
                {
                    if (!Manager.Assets.TryGetSpawnedObject(playerQueryKvp.Key, out LiteNetLibIdentity spawnedObject))
                        continue;
                    queryResult = playerQueryKvp.Value;
                    for (int i = 0; i < queryResult.Length; ++i)
                    {
                        uint contactedObjectId = queryResults[i].objectId;
                        if (!Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                        {
                            continue;
                        }
                        if (!ShouldSubscribe(foundPlayerObject, spawnedObject, false))
                        {
                            continue;
                        }
                        if (!_playerSubscribings.TryGetValue(contactedObjectId, out subscribings))
                            subscribings = _hashSetPool.Count > 0 ? _hashSetPool.Dequeue() : new HashSet<uint>();
                        subscribings.Add(spawnedObject.ObjectId);
                        _playerSubscribings[contactedObjectId] = subscribings;
                    }
                    ReturnResultList(queryResult);
                }
                _queryingPlayerSubscribings.Clear();

                foreach (var componentQueryKvp in _queryingComponentSubscribings)
                {
                    if (!SpatialObjectContainer.TryGet(componentQueryKvp.Key, out ISpatialObjectComponent component))
                        continue;
                    queryResult = componentQueryKvp.Value;
                    for (int i = 0; i < queryResult.Length; ++i)
                    {
                        uint contactedObjectId = queryResults[i].objectId;
                        if (Manager.Assets.TryGetSpawnedObject(contactedObjectId, out foundPlayerObject))
                            component.AddSubscriber(foundPlayerObject.ObjectId);
                    }
                    ReturnResultList(queryResult);
                }
                _queryingComponentSubscribings.Clear();

                var players = Manager.GetPlayers();
                while (players.MoveNext())
                {
                    LiteNetLibPlayer player = players.Current.Value;
                    if (!player.IsReady)
                    {
                        // Don't subscribe if player not ready
                        continue;
                    }
                    var playerObjs = player.GetSpawnedObjects();
                    while (playerObjs.MoveNext())
                    {
                        LiteNetLibIdentity playerObj = playerObjs.Current.Value;
                        if (_playerSubscribings.TryGetValue(playerObj.ObjectId, out subscribings))
                        {
                            if (_alwaysVisibleObjects.Count > 0)
                            {
                                foreach (uint alwaysVisibleObject in _alwaysVisibleObjects)
                                {
                                    subscribings.Add(alwaysVisibleObject);
                                }
                            }
                            playerObj.UpdateSubscribings(subscribings);
                            subscribings.Clear();
                        }
                        else if (_alwaysVisibleObjects.Count > 0)
                        {
                            playerObj.UpdateSubscribings(_alwaysVisibleObjects);
                        }
                        else
                        {
                            playerObj.ClearSubscribings();
                        }
                    }
                }
            }
        }

        private void FallbackUpdateInterestManagement(float deltaTime)
        {
            _updateCountDown -= deltaTime;
            if (_updateCountDown > 0)
                return;
            _updateCountDown = updateInterval;

            foreach (LiteNetLibPlayer player in Manager.GetPlayers())
            {
                if (!player.IsReady)
                    continue;
                foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                {
                    _fallbackSubscribings.Clear();
                    foreach (LiteNetLibIdentity spawnedObject in Manager.Assets.GetSpawnedObjects())
                    {
                        if (ShouldSubscribe(playerObject, spawnedObject))
                            _fallbackSubscribings.Add(spawnedObject.ObjectId);
                    }
                    playerObject.UpdateSubscribings(_fallbackSubscribings);
                }
            }

            foreach (ISpatialObjectComponent component in SpatialObjectContainer.GetValues())
            {
                if (component == null)
                    continue;
                component.ClearSubscribers();
                if (!component.SpatialObjectEnabled)
                    continue;
                foreach (LiteNetLibPlayer player in Manager.GetPlayers())
                {
                    if (!player.IsReady)
                        continue;
                    foreach (LiteNetLibIdentity playerObject in player.GetSpawnedObjects())
                    {
                        if (IsWithinSpatialShape(playerObject.transform.position, component))
                        {
                            component.AddSubscriber(playerObject.ObjectId);
                        }
                    }
                }
            }
        }

        private bool IsWithinSpatialShape(Vector3 objectPosition, ISpatialObjectComponent component)
        {
            switch (component.SpatialObjectShape)
            {
                case SpatialObjectShape.Box:
                    return IsWithinBox(
                        objectPosition,
                        (Vector3)component.SpatialObjectPosition,
                        (Vector3)component.SpatialObjectExtents);
                default:
                    return IsWithinSphere(
                        objectPosition,
                        (Vector3)component.SpatialObjectPosition,
                        component.SpatialObjectRadius);
            }
        }

        private bool IsWithinBox(Vector3 objectPosition, Vector3 center, Vector3 extents)
        {
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    return Mathf.Abs(objectPosition.x - center.x) <= extents.x &&
                        Mathf.Abs(objectPosition.y - center.y) <= extents.y;
                default:
                    return Mathf.Abs(objectPosition.x - center.x) <= extents.x &&
                        Mathf.Abs(objectPosition.z - center.z) <= extents.z;
            }
        }

        private bool IsWithinSphere(Vector3 objectPosition, Vector3 center, float radius)
        {
            float radiusSqr = radius * radius;
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    float xDiff2D = objectPosition.x - center.x;
                    float yDiff2D = objectPosition.y - center.y;
                    return xDiff2D * xDiff2D + yDiff2D * yDiff2D <= radiusSqr;
                default:
                    float xDiff3D = objectPosition.x - center.x;
                    float zDiff3D = objectPosition.z - center.z;
                    return xDiff3D * xDiff3D + zDiff3D * zDiff3D <= radiusSqr;
            }
        }
    }
}
