using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace UI.RadialMenu
{
    public class ButtonController : MonoBehaviour
    {
        //public int ID;
        //private Animator _animator;
        //[SerializeField] private string itemName;
        //public TextMeshProUGUI itemText;
        //private bool _selected = false;

        //private PlayerInputActions _controls;
        private InputAction _click;

        //void Awake()

        void Start()
        {
            //_animator = GetComponent<Animator>();
            //_controls = new InputAction();
            //_controls.UI.Click.performed += OnClick;
        }

        void Update()
        {

        }

        private void OnClick(InputAction.CallbackContext context)
        {
            Debug.Log("This button was clicked");
        }

        public void HoverEnter()
        {
            //_animator.SetBool("Hover", true);
            //itemText.text = itemName;
        }

        public void HoverExit()
        {
            //_animator.SetBool("Hover", false);
            //itemText.text = itemName;
        }
    }
}