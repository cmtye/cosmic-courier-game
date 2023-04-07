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
        
        // Update is called once per frame
        private void Update()
        {
            var targetPosition = target.transform.position;
            var moveDirection = (targetPosition - transform.position).normalized;

            transform.position += moveDirection * (_projectileSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < _damageDistance)
            {
                target.GetHealth().DealDamage(_projectileDamage, _damageType);
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
