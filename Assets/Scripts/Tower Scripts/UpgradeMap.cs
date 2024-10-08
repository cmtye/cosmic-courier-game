using System;
using System.Collections.Generic;
using Enemy_Scripts.Spawning_Scripts;
using Tower_Scripts.Components;
using UnityEngine;
using UX.RadialMenu;

namespace Tower_Scripts
{
    [CreateAssetMenu(fileName = "UpgradeMapTemplate.asset", menuName = "UpgradeMap")]
    public class UpgradeMap : ScriptableObject
    {
        [Serializable]
        public class Upgrade
        {
            // ReSharper disable once NotAccessedField.Local
            [SerializeField] private string upgradeTag;
            public TowerData tierStats;
            public List<TowerComponent> tierComponents;
            public GameObject tierVisuals;
            public Texture2D tierIcon;
            public Ring ringData;

        }

        [SerializeField] private Upgrade[] upgrades;
        public Upgrade[] Upgrades { get; private set; }
        
        private void OnEnable()
        {
            Upgrades = upgrades;
        }
    }
}
