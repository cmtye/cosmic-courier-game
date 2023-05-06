using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/SpreadPelletAttack")]
    public class SpreadPelletAttack : TowerComponent
    {
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private int pelletCount = 5;
        [SerializeField] private float spreadFactor = 0.01f;
        public override void Enabled(BaseTower tower)
        {
        }
        
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

            var lookRotation = Quaternion.LookRotation((tower.transform.position - tower.targetEnemy.transform.position).normalized);
            for(var i = 0; i < pelletCount; i++){
                
                var pelletRot = lookRotation;
                pelletRot.x += Random.Range(-spreadFactor, spreadFactor);
                pelletRot.y += Random.Range(-spreadFactor, spreadFactor);
                pelletRot.w += Random.Range(-spreadFactor, spreadFactor);
                pelletRot.z += Random.Range(-spreadFactor, spreadFactor);
                
                var pellet = Instantiate(projectilePrefab, tower.FiringPoint.position, pelletRot);
                var pelletBehavior = pellet.GetComponent<PelletBehavior>();
                pelletBehavior.SetParams(projectileSpeed, towerInfo.towerDamage, towerInfo.damageType);
                
            }

            tower.attackCooldown = tower.towerData.info.attackTimer;
        }
    }
}
