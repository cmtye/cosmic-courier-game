using System;
using System.Linq;
using Tower_Scripts;
using UnityEngine;

namespace Enemy_Scripts
{
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyHealthBehavior : MonoBehaviour
    {
        public static event Action<EnemyBehavior> OnEnemyKilled;
        public static event Action<EnemyBehavior> OnEnemyHit;
        
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform healthBarTransform;
        public float initialHealth = 10f;
        public float maxHealth = 10f;

        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        
        private EnemyBehavior _enemy;
        private Coroutine _transitionCoroutine;

        private void Start()
        {
            MaxHealth = maxHealth;
            CurrentHealth = initialHealth;
            CreateHealthBar();
            _enemy = GetComponent<EnemyBehavior>();
        }

        public void DealDamage(float damageReceived, ElementalTypes damageType)
        {
            if (_enemy.Invulnerabilities.Contains(damageType)) return;

            CurrentHealth -= damageReceived;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            else
            {
                OnEnemyHit?.Invoke(_enemy);
            }
        }
        
        private void CreateHealthBar()
        {
            var newBar = Instantiate(healthBarPrefab, healthBarTransform.position, 
                                               healthBarTransform.rotation, healthBarTransform.transform);
            var container = newBar.GetComponent<EnemyHealthContainer>();
            container.HealthBehavior = this;
        }

        private void Die()
        {
            OnEnemyKilled?.Invoke(_enemy);
        }
    }
}
