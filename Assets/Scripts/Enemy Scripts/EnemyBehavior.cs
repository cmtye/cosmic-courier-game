using System;
using System.Collections;
using System.Collections.Generic;
using Enemy_Scripts.Spawning_Scripts;
using Level_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Enemy_Scripts
{
    public enum ElementalTypes 
    { 
        Standard,
        Solar, 
        Lunar, 
        Storm, 
        Arcane
    }

    [RequireComponent(typeof(EnemyHealthBehavior))]
    public class EnemyBehavior : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private ElementalTypes[] vulnerableTo = { ElementalTypes.Standard };
        [SerializeField] private GameObject itemDrop;
        // [SerializeField] private AnimationCurve slowDownCurve;

        [HideInInspector] public ObjectPool parentPool;
        private EnemyHealthBehavior _healthBehavior;
        private List<Vector3> _path;
        private Coroutine _pathCoroutine;
        private Coroutine _moveCoroutine;

        public ElementalTypes[] Vulnerabilities { get; private set; }
        
        private void Start()
        {
            Vulnerabilities = vulnerableTo;
            _healthBehavior = GetComponent<EnemyHealthBehavior>();
            _path = PathManager.Instance.PathVectors;
            _pathCoroutine = StartCoroutine(MoveAlongPath());
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

            if (!itemDrop)
            {
                enemy.parentPool.ReturnToPool(enemy.gameObject);
                return;
            }
            
            if (Random.value > 0.2)
            {
                Instantiate(itemDrop, enemy.transform);
                var continuedDropChance = 0.2;
                while (Random.value > continuedDropChance)
                {
                    Instantiate(itemDrop, enemy.transform);
                    continuedDropChance += 0.2;
                }
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
                distanceToPoint = (position - _path[currentIndex]).magnitude;
                transform.position = position;
                yield return null;
            }
        }

        private void EndPointReached()
        {
            //OnEndReached?.Invoke(this);
            //_enemyHealth.ResetHealth();
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
