using TMPro;
using UnityEngine;

namespace UX
{

    public class StorageBehavior : MonoBehaviour
    {

        private TextMeshProUGUI _text;

        [SerializeField] private int currentValue = 0;

        private void Awake() 
        {
            _text = GetComponent<TextMeshProUGUI>();
        }


        public void SetCurrent(int value)
        {
            currentValue = value;
            UpdateText();
        }

        private void UpdateText()
        {
            //Debug.Log("Setting value to new number")
            _text.SetText(currentValue.ToString());
        }

        
    }
}