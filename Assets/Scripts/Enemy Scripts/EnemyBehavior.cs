using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Enemy_Scripts.Spawning_Scripts;
using Level_Scripts;
using Tower_Scripts;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy_Scripts
{
    [RequireComponent(typeof(EnemyHealthBehavior))]
    public class EnemyBehavior : MonoBehaviour
    {
        public static event Action<int> OnEndReached;
        
        public float moveSpeed = 5;
        [SerializeField] private ElementalTypes[] invulnerableTo = { };
        [SerializeField] private DropRates dropRates;
        // [SerializeField] private AnimationCurve slowDownCurve;

        [HideInInspector] public ObjectPool parentPool;
        private EnemyHealthBehavior _healthBehavior;
        private List<Vector3> _path;
        private Coroutine _pathCoroutine;
        private Coroutine _moveCoroutine;

        public ElementalTypes[] Invulnerabilities { get; private set; }
        
        private void Start()
        {
            Invulnerabilities = invulnerableTo;
            _healthBehavior = GetComponent<EnemyHealthBehavior>();
            _path = PathManager.Instance.PathVectors;
            _pathCoroutine = StartCoroutine(MoveAlongPath());
            dropRates.UpdateGrossRates();
        }
        
        private void OnEnable()
        {
            EnemyHealthBehavior.OnEnemyHit += HitStun;
            EnemyHealthBehavior.OnEnemyKilled += DropResources;
        }

        private void OnDisable()
        {
            EnemyHealthBehavior.OnEnemyHit -= HitStun;
            EnemyHealthBehavior.OnEnemyKilled -= DropResources;
        }

        private void DropResources(EnemyBehavior enemy)
        {
            if (enemy != this) return;
            
            StopCoroutine(enemy._moveCoroutine);
            StopCoroutine(enemy._pathCoroutine);

            // If no drop rates attached, return
            if (!dropRates)
            {
                enemy.parentPool.ReturnToPool(enemy.gameObject);
                return;
            }


            var rand = Random.value;
            GameObject itemToDrop = null;
            if (dropRates.SmallGrossRate > rand)
            {
                itemToDrop = dropRates.StardustSmallObject;
            }
            else if (dropRates.MediumGrossRate > rand)
            {
                itemToDrop = dropRates.StardustMediumObject;
            }
            else if (dropRates.LargeGrossRate > rand)
            {
                itemToDrop = dropRates.StardustLargeObject;
            } 
            else if (dropRates.DarkGrossRate > rand)
            {
                if (!GameManager.Instance.CheckDepotFull()) itemToDrop = dropRates.DarkMatterObject;
            }

            if (itemToDrop is not null)
            {
                Instantiate(itemToDrop, enemy.transform.position, Quaternion.identity);
            }
            enemy.parentPool.ReturnToPool(enemy.gameObject);
        }
        
        private void HitStun(EnemyBehavior enemy)
        {
            // Could be implemented as diminishing returns or as a towers unique ability
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
                
                var direction = (_path[currentIndex] - position).normalized;
                if (direction != Vector3.zero)
                {
                    var lookRotation = Quaternion.LookRotation((_path[currentIndex] - position).normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
                }
                distanceToPoint = (position - _path[currentIndex]).magnitude;
                transform.position = position;
                
                yield return null;
            }
        }

        private void EndPointReached()
        {
            OnEndReached?.Invoke(1);
            StopCoroutine(_moveCoroutine);
            StopCoroutine(_pathCoroutine);
            parentPool.ReturnToPool(gameObject);
        }

        public EnemyHealthBehavior GetHealth()
        {
            return _healthBehavior;
        }
    }
}
