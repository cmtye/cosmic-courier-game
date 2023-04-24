using Enemy_Scripts;
using UnityEngine;

namespace Tower_Scripts.Components
{
    public class BoomerangBehavior : MonoBehaviour
    {
        private Vector3 _initialPosition;

        private float _directionalScalar = .01f;

        private float _curveAccel = 1.001f;

        private float _rotationSpeed = 3f;

        private Vector3 _direction;
        private float _speed;
        private float _currentSpeed;
        private float _acceleration;
        private float _damage;
        private ElementalTypes _damageType;

        private bool _isLeft;

        public EnemyBehavior target;

        [SerializeField] private GameObject travelPrefab;
        [SerializeField] private GameObject hitPrefab;

        private void Start() 
        {
            _initialPosition = transform.position;

            _direction = (target.transform.position- transform.position).normalized;
            _direction.y = 0;
            Destroy(gameObject, .9f);
        }
        
        private void Update()
        {
            Move();

            Rotate();
        }


        private void Move()
        {
            _directionalScalar *= _curveAccel;

            if (_isLeft)
                _direction += (Vector3.Cross(_direction, Vector3.up) * _directionalScalar);
            else 
                _direction += (Vector3.Cross(_direction, Vector3.up) * -_directionalScalar);

            var directionToSun = transform.position - _initialPosition;

            _direction += (-directionToSun * .003f);

            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void Rotate()
        {
            transform.Rotate(0, _rotationSpeed, 0, Space.Self);        
        }

        private void OnTriggerEnter(Collider other)
        {
            var entity = other.gameObject;
            if (entity.CompareTag("Enemy"))
            {
                entity.GetComponent<EnemyHealthBehavior>().DealDamage(_damage, _damageType);
            }
        }

        public void SetParams(float speed, float acceleration, float damage, ElementalTypes damageType, bool isLeft)
        {
            _speed = speed;
            _acceleration = acceleration;
            _damage = damage;
            _damageType = damageType;
            _isLeft = isLeft;
        }
    }
}