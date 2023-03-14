using System;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy_Scripts
{
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyHealthBehavior : MonoBehaviour
    {
        public static event Action<EnemyBehavior> OnEnemyKilled;
        public static event Action<EnemyBehavior> OnEnemyHit;
        
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform barTransform;
        [SerializeField] private float initialHealth = 10f;

        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }

        private Image _healthBar;
        private EnemyBehavior _enemy;
        private Coroutine _transitionCoroutine;

        private void Start()
        {
            MaxHealth = 10f;
            CreateHealthBar();
            CurrentHealth = initialHealth;
            _enemy = GetComponent<EnemyBehavior>();
        }

        public void DealDamage(float damageReceived)
        {
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
            var newBar = Instantiate(healthBarPrefab, barTransform.position, Quaternion.identity, barTransform.transform);
            var container = newBar.GetComponent<EnemyHealthContainer>();
            container.HealthBehavior = this;
        }

        /*private void UpdateHealthBar()
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = StartCoroutine(UpdateHealth());
            }
            else
            {
                _transitionCoroutine = StartCoroutine(UpdateHealth());
            }
        }*/
        
        /*private IEnumerator UpdateHealth()
        {
            while(Math.Abs(_healthBar.fillAmount - CurrentHealth / MaxHealth) > 0.0001f) {
                _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, CurrentHealth / MaxHealth, Time.deltaTime * 10f);
                yield return null;
            }
        }*/

        private void Die()
        {
            OnEnemyKilled?.Invoke(_enemy);
        }
    }
}
