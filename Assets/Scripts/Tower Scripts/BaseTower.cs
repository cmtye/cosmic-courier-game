using System;
using System.Collections.Generic;
using System.Linq;
using Enemy_Scripts;
using Tower_Scripts.Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Tower_Scripts
{
    public enum ElementalTypes 
    { 
        Standard,
        Solar, 
        Lunar, 
        Storm, 
        Arcane
    }

    public enum TargetType
    {
        Close,
        Strong,
        Weak,
        Far
    }
    public class BaseTower : MonoBehaviour
    {
        public List<TowerComponent> towerComponents;
        public TowerData towerData;
        private CapsuleCollider _rangeCollider;
        private DecalProjector _decalProjector;
        public Transform firingPoint;
        
        public List<EnemyBehavior> enemiesInRange;
        
        public EnemyBehavior targetEnemy;
        private float _targetDistance;
        private float _targetHealth;
        
        public float attackCooldown;

        public void EnableComponents()
        {
            foreach (var c in towerComponents)
            {
                if (c) c.Enabled(this);
            }
        }

        public void UpdateTowerData(TowerData data)
        {
            towerData = data;

            if (_rangeCollider)
            {
                _rangeCollider.radius = towerData.info.towerRange;
            }

            if (_decalProjector)
            {
                _decalProjector.size = new Vector3(towerData.info.towerRange * 2, towerData.info.towerRange * 2, 4);
            }
        }
        
        private void OnEnable()
        {
            EnableComponents();
            _rangeCollider = GetComponent<CapsuleCollider>();
            if (_rangeCollider)
            {
                _rangeCollider.radius = towerData.info.towerRange;
            }

            _decalProjector = GetComponentInChildren<DecalProjector>();
            if (_decalProjector)
            {
                _decalProjector.size = new Vector3(towerData.info.towerRange * 2, towerData.info.towerRange * 2, 4);
            }
            attackCooldown = towerData.info.attackTimer;
        }

        private void Update()
        {
            enemiesInRange.RemoveAll(x => !x.isActiveAndEnabled);
            GetTargetEnemy();
            foreach (var c in towerComponents)
            {
                if (c) c.Execute(this);
            }
        }

        private void GetTargetEnemy()
        {
            if (enemiesInRange.Count == 0)
            {
                targetEnemy = null;
                return;
            }

            targetEnemy = towerData.info.targetType switch
            {
                TargetType.Close => enemiesInRange
                    .Where(e => e)
                    .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
                    .FirstOrDefault(),
                TargetType.Far => enemiesInRange
                    .Where(e => e)
                    .OrderByDescending(e => Vector3.Distance(e.transform.position, transform.position))
                    .FirstOrDefault(),
                TargetType.Strong => enemiesInRange
                    .Where(e => e)
                    .OrderByDescending(e => e.GetHealth().MaxHealth)
                    .FirstOrDefault(),
                TargetType.Weak => enemiesInRange
                    .Where(e => e)
                    .OrderBy(e => e.GetHealth().CurrentHealth)
                    .FirstOrDefault(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            var newEnemy = other.transform.parent.GetComponent<EnemyBehavior>();
            enemiesInRange.Add(newEnemy);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            
            var enemy = other.transform.parent.GetComponent<EnemyBehavior>();
            if (enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
