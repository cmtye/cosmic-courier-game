using System;
using UnityEngine;

namespace Utility_Scripts.RadialMenu
{
    public class ButtonInfo : ScriptableObject
    {
        public string Text;
        public Sprite Icon;

        public Action OnClick;
    }
}