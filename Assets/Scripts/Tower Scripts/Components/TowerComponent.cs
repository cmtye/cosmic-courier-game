using UnityEngine;

namespace Tower_Scripts.Components
{
    public abstract class TowerComponent : ScriptableObject
    {
        public abstract void Enabled(BaseTower tower);
        public abstract void Execute(BaseTower tower);
    }
}
