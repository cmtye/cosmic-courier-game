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
                if (t.gameObject.name != "Body") continue;
                
                foreach (Transform r in t.gameObject.transform)
                {
                    if (!r.CompareTag("Rotating")) continue;
                    _rotatingObjects[tower] = r;
                    break;
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

            if (!_rotatingObjects.TryGetValue(tower, out var rotator))
            {
                foreach (Transform t in tower.gameObject.transform)
                {
                    if (t.gameObject.name != "Body") continue;

                    foreach (Transform r in t.gameObject.transform)
                    {
                        if (!r.CompareTag("Rotating")) continue;
                        _rotatingObjects[tower] = r;
                        break;
                    }
                }
            }
            else
            {
                // Quaternions are weird, hard coded for saturn ring but should probably fix
                var lookRotation =
                    Quaternion.LookRotation((tower.firingPoint.position - tower.targetEnemy.transform.position).normalized);
                lookRotation *= Quaternion.Euler(0, 90, 0);
                rotator.rotation = lookRotation;
            }

            foreach (var e in tower.enemiesInRange)
                e.GetHealth().DealDamage(towerInfo.towerDamage, towerInfo.damageType);

            tower.attackCooldown = tower.towerData.info.attackTimer;
        }
    }
}
