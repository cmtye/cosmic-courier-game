using System;
using UnityEngine;

namespace Enemy_Scripts.Spawning_Scripts
{
    [CreateAssetMenu(fileName = "WaveMapTemplate.asset", menuName = "WaveMaps/WaveMap Template")]
    public class WaveMap : ScriptableObject
    {
        [Serializable]
        public class Wave
        {
            // ReSharper disable once NotAccessedField.Local
            [SerializeField] private string subWaveTag;
            public GameObject enemyPrefab;
            public int spawnCount;
            public float spawnDelay;
        }

        [SerializeField] private Wave[] subWaves;
        public Wave[] SubWaves { get; private set; }
        
        private void OnEnable()
        {
            SubWaves = subWaves;
        }
        
    }
}
