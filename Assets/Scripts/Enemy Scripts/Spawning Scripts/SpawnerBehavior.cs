using UnityEngine;

namespace Enemy_Scripts.Spawning_Scripts
{
    [RequireComponent(typeof(ObjectPool))]
    public class SpawnerBehavior : MonoBehaviour
    {
        [SerializeField] private int enemyCount = 10;
        [SerializeField] private float spawnDelay;

        private float _spawnTimer;
        private int _enemiesSpawned;

        private ObjectPool _pool;

        private void Start()
        {
            _pool = GetComponent<ObjectPool>();
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer < 0)
            {
                _spawnTimer = spawnDelay;
                if (_enemiesSpawned < enemyCount)
                {
                    _enemiesSpawned++;
                    SpawnEnemy();
                }
            }
        }

        private void SpawnEnemy()
        {
            var newEnemy = _pool.GetInstanceFromPool();
            newEnemy.SetActive(true);
        }
    }
}
