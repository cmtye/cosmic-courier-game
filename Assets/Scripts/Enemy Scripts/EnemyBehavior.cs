using System.Collections;
using System.Collections.Generic;
using Enemy_Scripts.Spawning_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility_Scripts;

namespace Enemy_Scripts
{
    public class EnemyBehavior : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5;
        private List<Vector3> _path;
        private Coroutine _moveCoroutine;

        private void Start()
        {
            _path = PathManager.Instance.PathVectors;
            StartCoroutine(MoveAlongPath());
        }
    
        private IEnumerator MoveAlongPath()
        {
            for (var i = 0; i < _path.Count; i++)
            {
                _moveCoroutine= StartCoroutine(Moving(i));
                yield return _moveCoroutine;
            }
            EndPointReached();
        }

        private IEnumerator Moving(int currentIndex)
        {
            var distanceToPoint = float.MaxValue;
            while (distanceToPoint > 0.01f)
            {
                var position = transform.position;
                position = Vector3.MoveTowards(position, _path[currentIndex] , moveSpeed * Time.deltaTime);
                distanceToPoint = (position - _path[currentIndex]).magnitude;
                transform.position = position;
                yield return null;
            }
        }

        private void EndPointReached()
        {
            //OnEndReached?.Invoke(this);
            //_enemyHealth.ResetHealth();
            //ObjectPool.ReturnToPool(gameObject);
        }
    }
}
