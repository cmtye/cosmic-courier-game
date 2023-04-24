using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/BoomerangAttack")]
    public class BoomerangAttack : TowerComponent
    {        

        public override void Enabled(BaseTower tower)
        {
        }

        [SerializeField] private GameObject boomerangPrefab;

        [SerializeField] private float initialSpeed = 20f;

        [SerializeField] private float returnAcceleration = .002f;
        [SerializeField] private bool isLeft;

        public override void Execute(BaseTower tower)
        {
            var towerInfo = tower.towerData.info;
            if (tower.attackCooldown > 0)
            {
                tower.attackCooldown -= Time.deltaTime;
                return;
            }
            
            if (!tower.targetEnemy)
            {
                return;
            }
            
            var projectile = Instantiate(boomerangPrefab, tower.FiringPoint.position, Quaternion.identity);
            var boomerangBehavior = projectile.GetComponent<BoomerangBehavior>();
            boomerangBehavior.target = tower.targetEnemy;
            boomerangBehavior.SetParams(initialSpeed, returnAcceleration, towerInfo.towerDamage, towerInfo.damageType, isLeft);
            
            tower.attackCooldown = tower.towerData.info.attackTimer;
        }

    }
}