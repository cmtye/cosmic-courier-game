using System.Collections.Generic;
using UnityEngine;

namespace Enemy_Scripts.Spawning_Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        private List<GameObject> _pool;
        private GameObject _poolContainer;
        private int _poolIndex;
        
        public static int ActiveInPool { get; private set; }

        public void CreatePool(string poolName)
        {
            ActiveInPool = 0;
            _poolIndex = 0;
            _pool = new List<GameObject>();
            if (_poolContainer)
                Destroy(_poolContainer);
            
            _poolContainer = new GameObject($"Pool - {poolName}");
        }

        public void AppendPool(GameObject prefab, int appendSize)
        {
            for (var i = 0; i < appendSize; i++)
            {
                _pool.Add(CreateInstance(prefab));
                ActiveInPool++;
            }
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var newInstance = Instantiate(prefab, _poolContainer.transform);
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

        public static void ReturnToPool(GameObject instance)
        {
            // Maybe some way to make this variable non-static?
            ActiveInPool--;
            instance.SetActive(false);
        }
    }
}
