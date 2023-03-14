using Enemy_Scripts.Spawning_Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace Enemy_Scripts
{
    public class EnemyBehavior : MonoBehaviour
    {
        /*public Vector3 CurrentPointPosition
        private void Update()
        {
            Move();
            if (CurrentPointReached())
            {
                UpdateCurrentPointIndex();
            }
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentPointPosition, MoveSpeed * Time.deltaTime);
        }

        private bool CurrentPointReached()
        {
            var distanceToPoint = (transform.position - CurrentPointPosition).magnitude;
            if (distanceToPoint < 0.1f)
            {
                _lastPointPosition = transform.position;
                return true;
            }
            return false;
            
        }

        private void UpdateCurrentPointIndex()
        {
            var lastWaypointIndex = Waypoint.Points.Length - 1;
            if (_currentWaypointIndex < lastWaypointIndex)
            {
                _currentWaypointIndex++;
            }
            else
            {
                EndPointReached();
            }
        }

        private void EndPointReached()
        {
            OnEndReached?.Invoke(this);
            _enemyHealth.ResetHealth();
            ObjectPool.ReturnToPool(gameObject);
        }*/
    }
}
