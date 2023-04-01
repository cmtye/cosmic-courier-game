using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Level_Scripts.Interaction;


namespace UI.RadialMenu
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

        private TextMeshProUGUI _selectionText;
        private RectTransform _rectTransform;

        private int _numButtons;

        private void Awake()
        {
            _selectionText = transform.Find("Selection Text").gameObject.GetComponent<TextMeshProUGUI>();
        }

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
                Pieces[i].Icon.transform.localPosition = Pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;
                Pieces[i].Icon.sprite = _data.buttons[i].Icon;

                // Set button data (text, icon, event) 
                Pieces[i].SetButtonData(_data.buttons[i]);

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

            for (int i = 0; i < _numButtons; i++)
            {
                Pieces[i].Recolor(false);
                if(i == hoveredIndex)
                {
                    Pieces[i].Recolor(true);
                    SetText(Pieces[i].GetText());
                    if (clicked)
                        Pieces[i].Execute(_player, _handler);
                }
            }

            if (!outer)
            {
                SetText("Cancel");
            }
            
            // If click -> exit
            if (clicked)
            {
                SetActive(false);
            }
        }

        private void SetText(string text)
        {
            _selectionText.SetText(text);
        }


        private float ModuloAngle(float a) => (a + 360f) % 360f;


    }
}
