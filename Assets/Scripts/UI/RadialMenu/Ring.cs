using UnityEngine;

namespace UI.RadialMenu
{
    [CreateAssetMenu(fileName = "Ring", menuName = "RadialMenu/Ring", order = 1)]
    public class Ring : ScriptableObject
    {
        public Button[] buttons;
    }
}
