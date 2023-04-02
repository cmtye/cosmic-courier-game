using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Enemy_Scripts.Spawning_Scripts
{
    [RequireComponent(typeof(ObjectPool))]
    public class SpawnManager : Singleton<SpawnManager>
    {
        public static event Action<int> OnWaveOver; 
        public WaveMap[] waves;
        private int _wavesIndex;
        
        [SerializeField] private Transform spawnPoint;
        
        private float _spawnDelay;
        private float _spawnTimer;

        private ObjectPool _pool;
        private WaitForSeconds _cacheWait;

        private void Start()
        {
            transform.position = spawnPoint.position;
            _wavesIndex = 0;
            _pool = GetComponent<ObjectPool>();
        }

        private void FixedUpdate()
        {
            if (_pool.ActiveInPool == 0) OnWaveOver?.Invoke(_wavesIndex);
        }

        public void StartNewWave()
        {
            if (_wavesIndex == waves.Length) return;
            // Potentially a better way to check current active in pool through events
            if (_pool.ActiveInPool != 0) return;
            
            _pool.CreatePool($"Wave {_wavesIndex + 1}");
            var subWaves = waves[_wavesIndex].SubWaves;
            foreach (var subWave in subWaves)
            {
                _pool.AppendPool(subWave.enemyPrefab, subWave.spawnCount);
            }
            StartCoroutine(BeginWave());

        }

        private void SpawnEnemy()
        {
            var newEnemy = _pool.GetInstanceFromPool();
            newEnemy.GetComponent<EnemyBehavior>().parentPool = _pool;
            newEnemy.transform.position = spawnPoint.position;
            newEnemy.SetActive(true);
        }
        
        private IEnumerator BeginWave()
        {
            var subWaves = waves[_wavesIndex].SubWaves;
            foreach (var subWave in subWaves)
            {
                _cacheWait = new WaitForSeconds(subWave.spawnDelay);
                for (var i = 0; i < subWave.spawnCount; i++)
                {
                    yield return _cacheWait;
                    SpawnEnemy();
                }
            }
            _wavesIndex++;
            
        }
    }
}
