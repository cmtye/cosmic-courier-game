using System;
using System.Collections.Generic;
using System.Linq;
using Enemy_Scripts;
using Interaction;
using Interaction.Events;
using Tower_Scripts.Components;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utility;
using UX;


namespace Tower_Scripts
{
    public enum ElementalTypes 
    { 
        Standard,
        Solar, 
        Lunar, 
        Storm, 
        Frost
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
        private IconUI _iconUI;
        public UpgradeMap towerUpgrades;
        public List<TowerComponent> towerComponents;
        public TowerData towerData;
        
        private CapsuleCollider _rangeCollider;
        private DecalProjector _decalProjector;
        public Transform FiringPoint { get; private set; }

        private Animation _animationHolder;
        private string[] _animationClips;
        private GameObject _towerBody;
        
        public List<EnemyBehavior> enemiesInRange;
        public List<ItemController> itemsInRange;
        
        public EnemyBehavior targetEnemy;
        private float _targetDistance;
        private float _targetHealth;
        
        public float attackCooldown;
        public bool IsDisabled { private get; set; }

        public Vector3Int cost;

        private TowerHandler _towerHandler;
        private int _targetingIndex;
        private string[] _targetingArray;

        private void Start()
        {
            enemiesInRange = new List<EnemyBehavior>();
            itemsInRange = new List<ItemController>();
            _towerHandler = GetComponent<TowerHandler>();
            _targetingArray = Enum.GetNames(typeof(TargetType));
            _targetingIndex = 0;

            SetTowerTier(0);
        }

        private void EnableComponents()
        {
            foreach (var c in towerComponents)
            {
                if (c) c.Enabled(this);
            }
        }

        private void SetTowerTier(int upgradeIndex)
        {
            var tierData = towerUpgrades.Upgrades[upgradeIndex];
            towerData = tierData.tierStats;
            towerComponents = tierData.tierComponents;
            var tempBody = _towerBody;
            var visuals = tierData.tierVisuals;
            _towerBody = Instantiate(visuals, transform);
            _towerBody.name = "Body";
            _iconUI.icon = tierData.tierIcon;
            _towerHandler.SetRingData(tierData.ringData);
            foreach (Transform t in _towerBody.transform)
            {
                if (t.name == "FiringPoint")
                {
                    FiringPoint = t;
                }
                if (t.name == "FiringPointContainer")
                {
                    FiringPoint = t.GetChild(0).transform;
                    foreach (Transform t2 in t.gameObject.transform)
                    {                
                        if (t.name == "FiringPoint")
                        {
                            FiringPoint = t;
                        }
                    }
                }
            }
            Destroy(tempBody);
            _animationHolder.clip = _animationHolder.GetClip(_animationClips[upgradeIndex]);
            _animationHolder.Play(_animationClips[upgradeIndex]);


            if (_towerHighlight)
            {
                _towerHighlight.needsRender = true;
            }
            
            EnableComponents();

            if (_rangeCollider) _rangeCollider.radius = towerData.info.towerRange;
            if (_decalProjector) _decalProjector.size = new Vector3(towerData.info.towerRange * 2, towerData.info.towerRange * 2, 4);
            
            attackCooldown = towerData.info.attackTimer;
        }
        
        private void OnEnable()
        {
            _rangeCollider = GetComponent<CapsuleCollider>();
            _decalProjector = GetComponentInChildren<DecalProjector>();
            _animationHolder = GetComponent<Animation>();
            _towerHighlight = GetComponent<OutlineHighlight>();
            _iconUI = GetComponent<IconUI>();
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

            TargetingEvent.OnTowerTargeting += CycleTargetingType;
            UpgradeEvent.OnTowerUpgrade += Upgrade;
        }

        private void OnDisable()
        {
            TargetingEvent.OnTowerTargeting -= CycleTargetingType;
            UpgradeEvent.OnTowerUpgrade -= Upgrade;
        }

        private void Update()
        {
            if (IsDisabled) return;
            enemiesInRange = enemiesInRange.Where(enemy => enemy).ToList();
            enemiesInRange.RemoveAll(x => !x.isActiveAndEnabled);
            itemsInRange = itemsInRange.Where(item => item).ToList();
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
            if (other.CompareTag("Item"))
            {
                var newItem = other.transform.GetComponentInChildren<ItemController>();
                if (!newItem || newItem.GetTier() == Item.DarkMatter) return;
                itemsInRange.Add(newItem);
            }
            if (!other.CompareTag("Enemy")) return;
            
            var newEnemy = other.transform.GetComponentInChildren<EnemyBehavior>();
            if (newEnemy.Invulnerabilities.Length != 0)
            {
                if (newEnemy.Invulnerabilities.Contains(towerData.info.damageType)) return;
            }
            enemiesInRange.Add(newEnemy);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                var item = other.transform.GetComponentInChildren<ItemController>();
                if (itemsInRange.Contains(item))
                {
                    itemsInRange.Remove(item);
                }
            }
            if (!other.CompareTag("Enemy")) return;
            
            var enemy = other.transform.GetComponentInChildren<EnemyBehavior>();
            if (enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }

        // The upgrade index is equivalent to the tier minus one, besides 3A and 3B which are index 2 and 3 respectively
        private void Upgrade(PlayerController player, InteractionHandler handler, int upgradeIndex, Vector3Int upgradeCost)
        { 
            // Only upgrade the tower instance accessed by the player
            if (handler.gameObject != gameObject) return;

            // If we cannot spend the cost of the upgrade, we can't upgrade
            if (!GameManager.Instance.Spend(upgradeCost)) return;
            
            // Cannot upgrade outside of the predefined tower bounds
            if (upgradeIndex > 3) return;
            
            SetTowerTier(upgradeIndex);
        }

        private void CycleTargetingType(PlayerController player, InteractionHandler handler)
        {
            // Only upgrade the tower instance accessed by the player
            if (handler.gameObject != gameObject) return;
            
            _targetingIndex++;
            if (_targetingIndex >= _targetingArray.Length) _targetingIndex = 0;

            towerData.info.targetType = _targetingArray[_targetingIndex] switch
            {
                "Close" => TargetType.Close,
                "Strong" => TargetType.Strong,
                "Weak" => TargetType.Weak,
                "Far" => TargetType.Far,
                _ => towerData.info.targetType
            };
        }

        public string CurrentTargeting()
        {
            return _targetingArray[_targetingIndex];
        }
    }
}
