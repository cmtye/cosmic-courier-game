using Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UX.RadialMenu
{
    public class MenuController : MonoBehaviour
    {
        private Ring _data;

        private PlayerController _player;
        private InteractionHandler _handler;
        public RingPiece RingCakePiecePrefab;
        public float GapWidthDegree = 1f;
        protected RingPiece[] Pieces;

        //protected MenuController Parent;

        [SerializeField] private TextMeshProUGUI selectionText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [SerializeField] private AudioClip click;

        private RectTransform _rectTransform;

        private int _numButtons;
        

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);
            
            Pieces = new RingPiece[0];
            SetText("");
            SetActive(false);
        }

        private void Update()
        {
            HandleMouse();
        }


        public void Setup(Ring ringData, PlayerController player, InteractionHandler handler)
        {
            Reset();
            _data = ringData;
            _player = player;
            _handler = handler;
            _numButtons = _data.buttons.Length;
            Generate();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }


        private void Reset()
        {
            foreach (var piece in Pieces)
            {
                Destroy(piece.gameObject);
            }

            _player = null;
            _handler = null;
        }

        private void Generate()
        {
            var stepLength = 360f / _numButtons;

            //Position it
            Pieces = new RingPiece[_numButtons];

            for (int i = 0; i < _numButtons; i++)
            {
                Pieces[i] = Instantiate(RingCakePiecePrefab, transform);
                // Set root element
                Pieces[i].transform.localPosition = Vector3.zero;
                Pieces[i].transform.localRotation = Quaternion.identity;

                // Set piece position
                Pieces[i].CakePiece.fillAmount = 1f / _numButtons - GapWidthDegree / 360f;
                Pieces[i].CakePiece.transform.localPosition = Vector3.zero;
                Pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f + GapWidthDegree / 2f + (i + 1) * stepLength);
                Pieces[i].Recolor(false);

                // Set icon position
                var iconDist = Vector3.Distance(RingCakePiecePrefab.Icon.transform.position, RingCakePiecePrefab.CakePiece.transform.position);
                var iconLocation = Pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;
                Pieces[i].Icon.transform.localPosition = iconLocation;
                Pieces[i].Icon.sprite = _data.buttons[i].Icon;

                // Set button data (text, icon, event) 
                Pieces[i].SetButtonData(_data.buttons[i]);

                // Set tooltip position
                Pieces[i].Tooltip.transform.localPosition = iconLocation + Vector3.down * 10;
                Pieces[i].Tooltip.GetComponent<TextMeshProUGUI>().SetText(_data.buttons[i].Tooltip);
            }
        }

        private void HandleMouse() 
        {
            var clicked = Mouse.current.leftButton.wasReleasedThisFrame;

            Vector3 mousePosition = Mouse.current.position.ReadValue(); 
            var stepLength = 360f / _numButtons;

            var transformPosition = transform.position;
            var mouseAngle = ModuloAngle(Vector3.SignedAngle(Vector3.up, mousePosition - transformPosition, Vector3.forward) + stepLength / 2f);
            var outer = (mousePosition - transformPosition).magnitude > transform.lossyScale.x * (_rectTransform.rect.width / 2.2);

            var hoveredIndex = outer ? (int)(mouseAngle / stepLength): -1;

            if (clicked)
                AudioManager.Instance.PlaySound(click, .1f);

            for (int i = 0; i < _numButtons; i++)
            {
                Pieces[i].Recolor(false);
                Pieces[i].ShowTooltip(false);
                if(i == hoveredIndex)
                {
                    Pieces[i].Recolor(true);
                    SetText(Pieces[i].GetText());
                    SetBlurbText(Pieces[i].GetBlurb());
                    Pieces[i].ShowTooltip(true);
                    if (clicked)
                        Pieces[i].Execute(_player, _handler);
                }
            }

            if (!outer)
            {
                SetText("Cancel");
                SetBlurbText("");
            }
            
            // If click -> exit
            if (clicked)
            {
                SetActive(false);
            }
        }

        private void SetText(string text)
        {
            selectionText.SetText(text);
        }
        
        private void SetBlurbText(string text)
        {
            descriptionText.SetText(text);
        }

        private float ModuloAngle(float a) => (a + 360f) % 360f;


    }
}
