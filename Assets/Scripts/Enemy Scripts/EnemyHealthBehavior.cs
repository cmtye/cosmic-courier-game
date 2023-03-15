using System;
using System.Linq;
using Enemy_Scripts.Spawning_Scripts;
using UnityEngine;

namespace Enemy_Scripts
{
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyHealthBehavior : MonoBehaviour
    {
        public static event Action<EnemyBehavior, EnemyHealthBehavior> OnEnemyKilled;
        public static event Action<EnemyBehavior, EnemyHealthBehavior> OnEnemyHit;
        
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform barTransform;
        [SerializeField] private float initialHealth = 10f;
        [SerializeField] private float maxHealth = 10f;

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

        public void DealDamage(float damageReceived, Vulnerability damageType)
        {
            // Vulnerability to normal attacks means vulnerability to everything
            if (!_enemy.Vulnerabilities.Contains(Vulnerability.Standard))
            {
                // Otherwise the enemy needs to be specifically vulnerable to be damaged
                if (!_enemy.Vulnerabilities.Contains(damageType)) return;
            }
            
            CurrentHealth -= damageReceived;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            else
            {
                OnEnemyHit?.Invoke(_enemy, this);
            }
        }
        
        private void CreateHealthBar()
        {
            var newBar = Instantiate(healthBarPrefab, barTransform.position, 
                                               barTransform.rotation, barTransform.transform);
            var container = newBar.GetComponent<EnemyHealthContainer>();
            container.HealthBehavior = this;
        }

        private void Die()
        {
            OnEnemyKilled?.Invoke(_enemy, this);
            ObjectPool.ReturnToPool(gameObject);
        }
    }
}
