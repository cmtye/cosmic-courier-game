using UnityEngine;
using TMPro;

namespace UI.RadialMenu
{
    public class RadialButtonController : MonoBehaviour
    {
        public int ID;
        private Animator _animator;
        [SerializeField] private string _text;
        public TextMeshProUGUI textDisplay;


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
            textDisplay.text = _text;
        }

        public void HoverExit()
        {
            _animator.SetBool("Hover", false);
            textDisplay.text = _text;
        }
    }
}