using System.Collections;
using UnityEngine;

namespace Enemy_Scripts.Spawning_Scripts
{
    [RequireComponent(typeof(ObjectPool))]
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private WaveMap[] waves;
        
        private float _spawnDelay;
        private float _spawnTimer;
        private int _wavesIndex;

        private ObjectPool _pool;
        private WaitForSeconds _cacheWait;

        private void Start()
        {
            _wavesIndex = 0;
            _pool = GetComponent<ObjectPool>();
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
