using System.Collections.Generic;
using UnityEngine;

namespace Tower_Scripts.Components
{
    [CreateAssetMenu(menuName = "Towers/Components/BuzzsawAttack")]
    public class BuzzsawAttack : TowerComponent
    {
        private Dictionary<BaseTower, Transform> _rotatingObjects;
        public override void Enabled(BaseTower tower)
        {
            _rotatingObjects ??= new Dictionary<BaseTower, Transform>();

            foreach (Transform t in tower.gameObject.transform)
            {
                if (t.CompareTag("Rotating"))
                {
                    _rotatingObjects[tower] = t;
                }
            }
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

            var rotator = _rotatingObjects[tower];
            // Quaternions are weird, hard coded for saturn ring but should probably fix
            var lookRotation =
                Quaternion.LookRotation((tower.firingPoint.position - tower.targetEnemy.transform.position).normalized);
            lookRotation *= Quaternion.Euler(0, 90, 0);
            rotator.rotation = lookRotation;
            
            foreach (var e in tower.enemiesInRange)
                e.GetHealth().DealDamage(towerInfo.towerDamage, towerInfo.damageType);

            tower.attackCooldown = tower.towerData.info.attackTimer;
        }
    }
}
