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

        private AudioClip damageSound;
        private AudioClip killSound;
        public float initialHealth = 10f;
        public float maxHealth = 10f;
        private ElementalTypes[] _elements;

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
            
            var stringEnums = Enum.GetNames(typeof(ElementalTypes));
            _elements = new ElementalTypes[stringEnums.Length - 1];
            var counter = 0;
            foreach (var s in stringEnums)
            {
                if (s == "Standard") continue;
                var parsedEnum = (ElementalTypes)Enum.Parse(typeof(ElementalTypes), s);
                _elements[counter] = parsedEnum;
                counter++;
            }
        }

        public void DealDamage(float damageReceived, ElementalTypes damageType)
        {
            if (_enemy.Invulnerabilities.Contains(damageType)) return;

            
            if (_enemy.Invulnerabilities.Length != 0)
            {
                // If the enemy has invulnerabilities, standard attacks should deal a fourth of the damage
                if (damageType == ElementalTypes.Standard)
                {
                    damageReceived /= 4;
                    if (damageReceived <= 0) damageReceived = 1;
                }
                // If the damage element is a counter, it should do double damage
                else
                {
                    for (var i = 0; i < _elements.Length - 1; i++)
                    {
                        if (_elements[i] != _enemy.Invulnerabilities[0]) continue;

                        var counterElement = i == _elements.Length - 1 ? 0 : i + 1;
                        if (damageType == _elements[counterElement])
                        {
                            damageReceived *= 2;
                        }
                    }
                }
            }
            
            CurrentHealth -= damageReceived;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            else
            {
                AudioManager.Instance.PlaySound(damageSound, .05f);
                OnEnemyHit?.Invoke(_enemy);
            }
        }
        
        private void CreateHealthBar()
        {
            var newBar = Instantiate(healthBarPrefab, healthBarTransform.position, 
                                               healthBarTransform.rotation, healthBarTransform.transform);
            var container = newBar.GetComponent<EnemyHealthContainer>();

            damageSound = container.damageSound;
            killSound = container.killSound;
 
            container.HealthBehavior = this;
        }

        private void Die()
        {
            AudioManager.Instance.PlaySound(killSound, .2f);
            OnEnemyKilled?.Invoke(_enemy);
        }
    }
}
