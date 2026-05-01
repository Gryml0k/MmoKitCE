using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseCacheManager<T, TCache>
        where TCache : BaseCacheData<T>, new()
    {
        public float cacheLifeTime = 30f;

        public abstract ProfilerMarker ProfilerMarker { get; }

        protected readonly Dictionary<string, TCache> _caches = new Dictionary<string, TCache>();

        // Reused by OnUpdate() so each cache sweep does not allocate a new List<string>
        // containing every dictionary key.
        private readonly List<string> _expiredKeys = new List<string>();

        public void OnUpdate()
        {
            if (_caches.Count <= 0)
                return;

            using (ProfilerMarker.Auto())
            {
                float time = Time.unscaledTime;
                _expiredKeys.Clear();

                foreach (KeyValuePair<string, TCache> entry in _caches)
                {
                    TCache cache = entry.Value;
                    if (cache == null || time - cache.TouchedTime >= cacheLifeTime)
                        _expiredKeys.Add(entry.Key);
                }

                for (int i = 0; i < _expiredKeys.Count; ++i)
                {
                    string key = _expiredKeys[i];
                    if (_caches.TryGetValue(key, out TCache cache))
                        cache?.Clear();
                    _caches.Remove(key);
                }

                _expiredKeys.Clear();
            }
        }

        public void Clear()
        {
            foreach (TCache cache in _caches.Values)
            {
                cache?.Clear();
            }

            _caches.Clear();
            _expiredKeys.Clear();
        }

        public TCache GetOrMakeCache(string id, in T data)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            if (!_caches.TryGetValue(id, out TCache cacheData) || cacheData == null)
            {
                cacheData = new TCache();
                _caches[id] = cacheData;
            }

            cacheData.Prepare(in data);
            return cacheData;
        }
    }
}
