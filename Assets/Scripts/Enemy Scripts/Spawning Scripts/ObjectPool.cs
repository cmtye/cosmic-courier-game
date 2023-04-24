using System;
using System.Collections.Generic;
using Tower_Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy_Scripts.Spawning_Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        private List<GameObject> _pool;
        private GameObject _poolContainer;
        private int _poolIndex;
        /*private ElementalTypes[] _invulnerablePossible;*/
        
        public int ActiveInPool { get; private set; }

        private void Start()
        {
            /*var stringEnums = Enum.GetNames(typeof(ElementalTypes));
            _invulnerablePossible = new ElementalTypes[stringEnums.Length - 1];
            var counter = 0;
            foreach (var s in stringEnums)
            {
                if (s == "Standard") continue;
                var parsedEnum = (ElementalTypes)Enum.Parse(typeof(ElementalTypes), s);
                _invulnerablePossible[counter] = parsedEnum;
                counter++;
            }*/
        }

        public void CreatePool(string poolName)
        {
            ActiveInPool = 0;
            _poolIndex = 0;
            _pool = new List<GameObject>();
            if (_poolContainer)
                Destroy(_poolContainer);
            
            _poolContainer = new GameObject($"Pool - {poolName}");
        }

        public void AppendPool(GameObject prefab, int appendSize, int prestigeLevel)
        {
            for (var i = 0; i < appendSize; i++)
            {
                _pool.Add(CreateInstance(prefab, prestigeLevel));
                ActiveInPool++;
            }
        }

        private GameObject CreateInstance(GameObject prefab, int prestigeLevel)
        {
            var newInstance = Instantiate(prefab, _poolContainer.transform);
            var enemyHealth = newInstance.GetComponent<EnemyHealthBehavior>();
            if (enemyHealth)
            {
                enemyHealth.maxHealth *= prestigeLevel;
                enemyHealth.initialHealth *= prestigeLevel;
                var enemy = enemyHealth.GetComponent<EnemyBehavior>();
                enemy.moveSpeed *= prestigeLevel;

                // Doesn't work since we don't have an easy way to change enemy texture
                /*if (prestigeLevel > 1)
                {
                    for (var i = 0; i < prestigeLevel; i++)
                    {
                        var rand = Random.value;
                        if (enemy.Invulnerabilities.Length != 0 || !(0.2f > rand)) continue;
                        
                        var newInvulnerability = _invulnerablePossible[Random.Range(0, _invulnerablePossible.Length)];
                        enemy.Invulnerabilities = new[] { newInvulnerability };
                    }
                }*/
            }
            newInstance.SetActive(false);
            return newInstance;
        }

        public GameObject GetInstanceFromPool()
        {
            if (!_pool[_poolIndex].activeInHierarchy)
            {
                var requestedInstance = _pool[_poolIndex];
                _poolIndex++;
                return requestedInstance;
            }
            return null;
        }

        public void ReturnToPool(GameObject instance)
        {
            // Maybe some way to make this variable non-static?
            ActiveInPool--;
            instance.SetActive(false);
        }
    }
}
