using UnityEngine;
using TMPro;

namespace UI.RadialMenu
{
    public class ButtonContoller : MonoBehaviour
    {
        public int ID;
        private Animator _animator;
        [SerializeField] private string itemName;
        public TextMeshProUGUI itemText;
        private bool _selected = false;


        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {

        }

        public void HoverEnter()
        {
            _animator.SetBool("Hover", true);
            itemText.text = itemName;
        }

        public void HoverExit()
        {
            _animator.SetBool("Hover", false);
            itemText.text = itemName;
        }
    }
}