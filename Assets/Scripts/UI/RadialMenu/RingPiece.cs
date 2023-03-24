using UnityEngine.UI;
using UnityEngine;

namespace UI.RadialMenu
{
    public class RingPiece : MonoBehaviour
    {

        public Image CakePiece;

        private Button _button;

        public Image Icon;


        private Color _selectedColor = new Color(1f, 1f, 1f, 0.4f);
        private Color _baseColor = new Color(1f, 1f, 1f, 0.8f);


        public void Recolor(bool selected)
        {
            CakePiece.color = selected ? _selectedColor : _baseColor;
        }

        public string GetText()
        {
            return _button.Text;
        }

        public void SetButtonData(Button button)
        {
            _button = button;
        }

        public void Execute()
        {
            _button.Event.Raise();
        }

    }
}