using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Yajima
{
    public class UnfinishedPool : IngameBehaviour
    {
        [SerializeField] Dictionary<UnfinishedType, Unfinished> _unfinishedPrefabs = new();
        [SerializeField] UnfinishedTier[] _unfinishedTier;
        [SerializeField] SplineContainer _spawnSplineContainer;
        [SerializeField, Min(0)] int _maxSpawnCount = 30;
        [SerializeField, Min(0f)] float _spawnInterval = 0.1f;

        readonly List<PooledObject> _objects = new();
        Transform _inactiveRoot;
        double _nextSpawnTime;
        double _spawnClock;
        UnfinishedType _selectedType;

        public UnfinishedType SelectedType => _selectedType;

        public event Action<int> OnInteractedPlayer;

        private void Update()
        {
            if (IsStopped) return;

            _spawnClock += Time.deltaTime;
            TrySpawn(out _);
        }

        public bool TrySpawn(out Unfinished unfinished)
        {
            unfinished = null;
            if (IsStopped || _spawnClock < _nextSpawnTime || _unfinishedPrefabs == null) return false;

            int tierIndex = _unfinishedTier is { Length: > 0 }
                ? UnityEngine.Random.Range(0, _unfinishedTier.Length)
                : -1;
            return TrySpawn(_selectedType, tierIndex, out unfinished);
        }

        public void SelectStartingType()
        {
            _selectedType = UnfinishedType.Default;
            if (_unfinishedPrefabs == null) return;

            int validCount = 0;
            foreach (UnfinishedType type in Enum.GetValues(typeof(UnfinishedType)))
            {
                if (type == UnfinishedType.Default || GetPrefab(type) == null) continue;
                if (UnityEngine.Random.Range(0, ++validCount) == 0) _selectedType = type;
            }
        }

        Unfinished GetPrefab(UnfinishedType type)
        {
            if (_unfinishedPrefabs.TryGetValue(type, out var prefab) && prefab != null)
                return prefab;

            return _unfinishedPrefabs.TryGetValue(UnfinishedType.Default, out var fallback) ? fallback : null;
        }

        // tierIndex が -1 の場合はプレハブの初期値を使用する。
        public bool TrySpawn(UnfinishedType type, int tierIndex, out Unfinished unfinished)
        {
            unfinished = null;
            if (!isActiveAndEnabled || IsStopped || _spawnClock < _nextSpawnTime || _maxSpawnCount <= 0 || _unfinishedPrefabs == null
                || tierIndex < -1 || (tierIndex >= 0 && (_unfinishedTier == null
                    || tierIndex >= _unfinishedTier.Length || _unfinishedTier[tierIndex] == null)))
                return false;

            var prefab = GetPrefab(type);
            if (prefab == null) return false;

            _objects.RemoveAll(item => item.Instance == null);
            if (_objects.Count >= _maxSpawnCount && !_objects.Exists(item => !item.InUse)) return false;

            var spline = FindSpawnSpline();
            if (spline == null) return false;

            var item = GetObject(type, prefab);
            if (item == null) return false;

            unfinished = item.Instance;
            item.InUse = true;
            item.SpawnSpline = spline;
            item.SpawnPosition = Unfinished.GetSplineEndpoint(_spawnSplineContainer, spline, 0);
            unfinished.transform.SetPositionAndRotation(item.SpawnPosition, _spawnSplineContainer.transform.rotation);
            unfinished.SetMoveSpline(_spawnSplineContainer, spline);

            var tier = tierIndex >= 0 ? _unfinishedTier[tierIndex] : null;
            if (tier != null)
                tier.ApplyTo(unfinished);
            else
                unfinished.UseDefaultSettings(prefab);

            _nextSpawnTime = _spawnClock + _spawnInterval;
            unfinished.gameObject.SetActive(true);
            unfinished.Entry();
            return true;
        }

        Spline FindSpawnSpline()
        {
            if (_spawnSplineContainer == null) return null;

            Spline selected = null;
            int validCount = 0;
            foreach (var spline in _spawnSplineContainer.Splines)
            {
                if (spline == null || spline.Count < 2) continue;

                var start = Unfinished.GetSplineEndpoint(_spawnSplineContainer, spline, 0);
                if (_objects.Exists(item => item.InUse
                    && (item.SpawnSpline == spline || item.SpawnPosition.Equals(start)))) continue;

                if (UnityEngine.Random.Range(0, ++validCount) == 0) selected = spline;
            }

            return selected;
        }

        PooledObject GetObject(UnfinishedType type, Unfinished prefab)
        {
            var reusable = _objects.Find(item => !item.InUse && item.Type == type && item.Prefab == prefab);
            if (reusable != null) return reusable;

            if (_objects.Count >= _maxSpawnCount)
            {
                var unused = _objects.Find(item => !item.InUse);
                if (unused == null) return null;

                unused.Instance.OnExit -= Return;
                unused.Instance.OnInteractedPlayer -= ForwardInteraction;
                _objects.Remove(unused);
                Destroy(unused.Instance.gameObject);
            }

            if (_inactiveRoot == null)
            {
                var root = new GameObject("Unfinished Pool Inactive");
                root.SetActive(false);
                _inactiveRoot = root.transform;
                _inactiveRoot.SetParent(transform, false);
            }

            var instance = Instantiate(prefab, _inactiveRoot);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(transform, false);
            instance.OnExit += Return;
            instance.OnInteractedPlayer += ForwardInteraction;

            var created = new PooledObject { Type = type, Prefab = prefab, Instance = instance };
            _objects.Add(created);
            return created;
        }

        void ForwardInteraction(int score)
        {
            OnInteractedPlayer?.Invoke(score);
        }

        void Return(Unfinished unfinished)
        {
            var item = _objects.Find(candidate => candidate.Instance == unfinished);
            if (item == null || !item.InUse) return;

            unfinished.gameObject.SetActive(false);
            item.SpawnSpline = null;
            item.InUse = false;
        }

        [Serializable]
        class UnfinishedTier
        {
            [SerializeField, Min(0f)] float _lifeTime = 5f;
            [SerializeField, Min(0f)] float _moveSpeed = 3f;
            [SerializeField, Min(0)] int _score = 100;

            public void ApplyTo(Unfinished unfinished)
            {
                unfinished.Init(_score, _lifeTime, _moveSpeed);
            }
        }

        class PooledObject
        {
            public UnfinishedType Type;
            public Unfinished Prefab;
            public Unfinished Instance;
            public Spline SpawnSpline;
            public Vector3 SpawnPosition;
            public bool InUse;
        }
    }
}
