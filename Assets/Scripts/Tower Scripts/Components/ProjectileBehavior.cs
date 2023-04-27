using Enemy_Scripts;
using UnityEngine;

namespace Tower_Scripts.Components
{
    public class ProjectileBehavior : MonoBehaviour
    {
        private float _projectileSpeed;
        private float _projectileDamage;
        private ElementalTypes _damageType;
        private float _damageDistance;

        public EnemyBehavior target;

        [SerializeField] private GameObject travelPrefab;
        [SerializeField] private GameObject hitPrefab;
        [SerializeField] private float damageModifier = 1f;
        
        // Update is called once per frame
        private void Update()
        {
            if (!target.isActiveAndEnabled)
            {
                Destroy(gameObject);
                return;
            }
            
            var targetPosition = target.transform.position;
            
            var position = transform.position;
            var moveDirection = (targetPosition - position).normalized;

            position += moveDirection * (_projectileSpeed * Time.deltaTime);
            transform.position = position;

            if (Vector3.Distance(position, targetPosition) < _damageDistance)
            {
                if (!target.isActiveAndEnabled)
                {
                    Destroy(gameObject);
                    return;
                }
                
                target.GetHealth().DealDamage(_projectileDamage * damageModifier, _damageType);
                Destroy(gameObject);
            }
        }

        public void SetParams(float speed, float damage, ElementalTypes damageType, float destroyDistance)
        {
            _projectileSpeed = speed;
            _projectileDamage = damage;
            _damageType = damageType;
            _damageDistance = destroyDistance;
        }
    }
}
