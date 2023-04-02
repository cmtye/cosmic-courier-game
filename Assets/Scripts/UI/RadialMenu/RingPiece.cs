using UnityEngine.UI;
using UnityEngine;
using Level_Scripts.Interaction;
using TMPro;

namespace UI.RadialMenu
{
    public class RingPiece : MonoBehaviour
    {

        public Image CakePiece;

        private Button _button;

        public TextMeshProUGUI Tooltip;

        public Image Icon;


        private Color _selectedColor = new (.01f, .36f, .61f, 0.6f);
        private Color _baseColor = new (0f, 0f, 0f, 0.7f);


        public void Recolor(bool selected)
        {
            CakePiece.color = selected ? _selectedColor : _baseColor;
        }

        public string GetText()
        {
            return _button.Text;
        }


        public void ShowTooltip(bool show)
        {
            Tooltip.enabled = show;
        }

        public void SetButtonData(Button button)
        {
            _button = button;
        }

        public void Execute(PlayerController player, InteractionHandler handler)
        {
            _button.Event.Raise(player, handler);
        }

    }
}