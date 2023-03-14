using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy_Scripts.Spawning_Scripts
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int poolSize = 10;
        private List<GameObject> _pool;
        private GameObject _poolContainer;

        private void Awake()
        {
            _pool = new List<GameObject>();
            _poolContainer = new GameObject($"Pool - {prefab.name}");
            CreatePool();
        }

        private void CreatePool()
        {
            for (var i = 0; i < poolSize; i++)
            {
                _pool.Add(CreateInstance());
            }
        }

        private GameObject CreateInstance()
        {
            var newInstance = Instantiate(prefab, _poolContainer.transform);
            newInstance.SetActive(false);
            return newInstance;
        }

        public GameObject GetInstanceFromPool()
        {
            for (var i = 0; i < _pool.Count; i++)
            {
                if (!_pool[i].activeInHierarchy)
                {
                    return _pool[i];
                }
            }
            return CreateInstance();
        }

        public static void ReturnToPool(GameObject instance)
        {
            instance.SetActive(false);
        }

        public static IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
        {
            yield return new WaitForSeconds(delay);
            instance.SetActive(false);
        }
    }
}
