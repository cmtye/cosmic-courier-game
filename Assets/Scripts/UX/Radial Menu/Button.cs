using Interaction.Events;
using UnityEngine;

namespace UX.RadialMenu
{
    [CreateAssetMenu(fileName = "Button", menuName = "RadialMenu/Button", order = 2)]
    public class Button : ScriptableObject
    {
        public string Text;
        public string Blurb;
        public string Tooltip;
        public Sprite Icon;
        public InteractionEvent Event;
    }
}
