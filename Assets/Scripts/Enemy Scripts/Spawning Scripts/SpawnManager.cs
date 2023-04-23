using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Utility;
using UX;

namespace Enemy_Scripts.Spawning_Scripts
{
    [RequireComponent(typeof(ObjectPool))]
    public class SpawnManager : Singleton<SpawnManager>
    {
        public static event Action<int> OnWaveOver; 
        public WaveMap[] waves;
        private int _wavesIndex;
        private int _prestigeLevel;
        private bool _autoStart;
        
        [SerializeField] private Transform spawnPoint;
        
        private float _spawnDelay;
        private float _spawnTimer;

        private ObjectPool _pool;
        private WaitForSeconds _cacheWait;

        [SerializeField] private GameObject autoButton;
        [SerializeField] private GameObject nextButton;
        [SerializeField] private TextMeshProUGUI waveTimerText;
        [SerializeField] private int waveTimer;
        private bool _waveWaiting;
        private Coroutine _waveWaitCoroutine;

        private void Start()
        {
            transform.position = spawnPoint.position;
            _wavesIndex = 0;
            _prestigeLevel = 1;
            _pool = GetComponent<ObjectPool>();
        }

        public void ToggleAutoStart()
        {
            _autoStart = !_autoStart;
        }
        
        private void FixedUpdate()
        {
            if (_waveWaiting) return;
            
            if (_pool.ActiveInPool == 0)
            {
                OnWaveOver?.Invoke(_wavesIndex + waves.Length * (_prestigeLevel - 1));
                if (_waveWaitCoroutine != null)
                {
                    StopCoroutine(_waveWaitCoroutine);
                    _waveWaitCoroutine = null;
                }
                if (_autoStart)
                {
                    StartNewWave();
                }
                else
                {
                    waveTimerText.enabled = true;
                    _waveWaiting = true;
                    nextButton.SetActive(true);
                    autoButton.SetActive(false);
                    _waveWaitCoroutine = StartCoroutine(WaveWait());
                }
            }
        }

        private IEnumerator WaveWait()
        {
            for (var i = waveTimer; i > 0; i--)
            {
                waveTimerText.text = i + "";
                yield return new WaitForSeconds(1);
            }
            StartNewWave();
        }
        
        public void StartNewWave()
        {
            nextButton.GetComponent<ObjectButtonBehavior>().SetInactive();
            autoButton.SetActive(true);
            waveTimerText.enabled = false;
            _waveWaiting = false;
            
            if (_wavesIndex == waves.Length)
            {
                _wavesIndex = 0;
                _prestigeLevel++;
            }
            
            if (_pool.ActiveInPool != 0) return;
            
            _pool.CreatePool($"Wave {_wavesIndex + waves.Length * (_prestigeLevel - 1) + 1}");
            var subWaves = waves[_wavesIndex].SubWaves;
            foreach (var subWave in subWaves)
            {
                _pool.AppendPool(subWave.enemyPrefab, subWave.spawnCount * _prestigeLevel, _prestigeLevel);
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
                for (var i = 0; i < subWave.spawnCount * _prestigeLevel; i++)
                {
                    yield return _cacheWait;
                    SpawnEnemy();
                }
            }
            _wavesIndex++;
            
        }
    }
}
