using UnityEngine.UI;
using UnityEngine;

namespace UI.RadialMenu
{
    public class RingPiece : MonoBehaviour
    {
        public Image Icon;
        public Image CakePiece;

        public string Text;

        private Color _selectedColor = new Color(1f, 1f, 1f, 0.75f);
        private Color _baseColor = new Color(1f, 1f, 1f, 0.5f);


        void Start() 
        {
        }

        public void Recolor(bool selected)
        {
            CakePiece.color = selected ? _selectedColor : _baseColor;
        }


        public void Execute()
        {

        }


    }
}