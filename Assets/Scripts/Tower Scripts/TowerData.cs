using System;
using Enemy_Scripts;
using UnityEngine;

namespace Tower_Scripts
{
    [CreateAssetMenu(fileName = "TowerDataTemplate.asset", menuName = "Towers/TowerData Template")]
    public class TowerData : ScriptableObject
    {
        [Serializable]
        public class Data
        {
            // ReSharper disable once NotAccessedField.Local
            [SerializeField] private string towerTag;
            public float towerRange = 2f;
            public float towerDamage = 2f;
            public float attackTimer = 2f;
            public ElementalTypes damageType;
            public TargetType targetType;
        }
        public Data info;
        
    }
}
