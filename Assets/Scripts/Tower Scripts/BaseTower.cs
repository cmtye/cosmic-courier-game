using System;
using System.Collections.Generic;
using Enemy_Scripts;
using Tower_Scripts.Components;
using UnityEngine;

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
            _rangeCollider.radius = towerData.info.towerRange;
        }
        
        private void OnEnable()
        {
            EnableComponents();
            _rangeCollider = GetComponent<CapsuleCollider>();
            _rangeCollider.radius = towerData.info.towerRange;
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
            switch (towerData.info.targetType)
            {
                case TargetType.Close:
                {
                    if (enemiesInRange.Count == 0)
                    {
                        targetEnemy = null;
                        break;
                    }
                    _targetDistance = float.MaxValue;
                    
                    foreach (var e in enemiesInRange)
                    {
                        if (!e) continue;
                        
                        var enemyDistance = Vector3.Distance(e.transform.position, transform.position);
                        if (enemyDistance < _targetDistance)
                        {
                            targetEnemy = e;
                            _targetDistance = enemyDistance;
                        }
                    }
                    break;
                }
                case TargetType.Far:
                {
                    if (enemiesInRange.Count == 0)
                    {
                        targetEnemy = null;
                        break;
                    }
                    _targetDistance = 0;

                    foreach (var e in enemiesInRange)
                    {
                        if (!e) continue;
                        var enemyDistance = Vector3.Distance(e.transform.position, transform.position);
                        if (enemyDistance > _targetDistance)
                        {
                            targetEnemy = e;
                            _targetDistance = enemyDistance;
                        }
                    }
                    break;
                }
                case TargetType.Strong:
                {
                    if (enemiesInRange.Count == 0)
                    {
                        targetEnemy = null;
                        break;
                    }
                    _targetHealth = 0;
                    
                    foreach (var e in enemiesInRange)
                    {
                        if (!e) continue;
                        var enemyMaxHealth = e.GetHealth().MaxHealth;
                        if (enemyMaxHealth > _targetHealth)
                        {
                            targetEnemy = e;
                            _targetHealth = enemyMaxHealth;
                        }
                    }
                    break;
                }
                case TargetType.Weak:
                {
                    if (enemiesInRange.Count == 0)
                    {
                        targetEnemy = null;
                        break;
                    }
                    _targetHealth = float.MaxValue;
                    
                    foreach (var e in enemiesInRange)
                    {
                        if (!e) continue;
                        var enemyCurrentHealth = e.GetHealth().CurrentHealth;
                        if (enemyCurrentHealth < _targetHealth)
                        {
                            targetEnemy = e;
                            _targetHealth = enemyCurrentHealth;
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
