using UnityEngine;

namespace UI.RadialMenu
{
    [CreateAssetMenu(fileName = "Button", menuName = "RadialMenu/Button", order = 2)]
    public class Button : ScriptableObject
    {
        public string Text;
        public Sprite Icon;
        public InteractionEvent Event;
    }
}
