using System;
using System.Collections.Generic;
using System.Linq;
using Enemy_Scripts;
using Tower_Scripts.Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Level_Scripts.Interaction;
using Utility;


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
        private OutlineHighlight _towerHighlight;
        public UpgradeMap towerUpgrades;
        private int _upgradeIndex;
        public List<TowerComponent> towerComponents;
        public TowerData towerData;
        
        private CapsuleCollider _rangeCollider;
        private DecalProjector _decalProjector;
        public Transform FiringPoint { get; private set; }

        private Animation _animationHolder;
        private string[] _animationClips;
        private GameObject _towerBody;
        
        public List<EnemyBehavior> enemiesInRange;
        
        public EnemyBehavior targetEnemy;
        private float _targetDistance;
        private float _targetHealth;
        
        public float attackCooldown;
        public bool IsDisabled { private get; set; }

        private void EnableComponents()
        {
            foreach (var c in towerComponents)
            {
                if (c) c.Enabled(this);
            }
        }

        private void IncrementTowerTier()
        {
            towerData = towerUpgrades.Upgrades[_upgradeIndex].tierStats;
            towerComponents = towerUpgrades.Upgrades[_upgradeIndex].tierComponents;
            var tempBody = _towerBody;
            var visuals = towerUpgrades.Upgrades[_upgradeIndex].tierVisuals;
            _towerBody = Instantiate(visuals, transform);
            _towerBody.name = "Body";
            foreach (Transform t in _towerBody.transform)
            {
                if (t.name == "FiringPoint")
                {
                    FiringPoint = t;
                }
            }
            Destroy(tempBody);
            _animationHolder.clip = _animationHolder.GetClip(_animationClips[_upgradeIndex]);
            _animationHolder.Play(_animationClips[_upgradeIndex]);


            if (_towerHighlight)
            {
                _towerHighlight.needsRender = true;
            }
            
            EnableComponents();

            if (_rangeCollider) _rangeCollider.radius = towerData.info.towerRange;
            if (_decalProjector) _decalProjector.size = new Vector3(towerData.info.towerRange * 2, towerData.info.towerRange * 2, 4);
            
            attackCooldown = towerData.info.attackTimer;
            _upgradeIndex++;
        }
        
        private void OnEnable()
        {
            _rangeCollider = GetComponent<CapsuleCollider>();
            _decalProjector = GetComponentInChildren<DecalProjector>();
            _animationHolder = GetComponent<Animation>();
            _towerHighlight = GetComponent<OutlineHighlight>();
            _animationClips = new string[4];

            var clipAmount = 0;
            foreach (AnimationState state in _animationHolder) 
            {
                _animationClips[clipAmount] = state.name;
                clipAmount++;
            }
            
            foreach (Transform t in transform)
            {
                if (t.CompareTag("TowerBody")) _towerBody = t.gameObject;
            }
            IncrementTowerTier();

            UpgradeEvent.OnTowerUpgrade += Upgrade;
        }

        private void OnDisable()
        {
            UpgradeEvent.OnTowerUpgrade -= Upgrade;
        }

        private void Update()
        {
            if (IsDisabled) return;
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

        private void Upgrade(PlayerController player, InteractionHandler handler)
        { 
            if (handler.gameObject != gameObject) return;
            IncrementTowerTier();
        }
    }
}
