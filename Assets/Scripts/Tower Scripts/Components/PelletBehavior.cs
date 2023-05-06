using Enemy_Scripts;
using UnityEngine;

namespace Tower_Scripts.Components
{
    public class PelletBehavior : MonoBehaviour
    {
        private Vector3 _direction;
        private float _movementSpeed;
        private float _damage;
        private ElementalTypes _damageType;
        

        [SerializeField] private GameObject travelPrefab;
        [SerializeField] private GameObject hitPrefab;
        [SerializeField] private float damageModifier = 1f;

        private void Start()
        {
            _direction = transform.forward;
            Destroy(gameObject, .95f);
        }
        
        private void Update()
        {
            transform.position -= _direction * Time.deltaTime * _movementSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            var entity = other.gameObject;
            if (entity.CompareTag("Enemy"))
            {
                entity.GetComponent<EnemyHealthBehavior>().DealDamage(_damage * damageModifier, _damageType);
            }
        }

        public void SetParams(float speed, float damage, ElementalTypes damageType)
        {
            _movementSpeed = speed;
            _damage = damage;
            _damageType = damageType;
        }
    }
}
