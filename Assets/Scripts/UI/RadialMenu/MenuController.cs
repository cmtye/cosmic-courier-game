using UnityEngine;

namespace UI.RadialMenu
{
    public class MenuController : MonoBehaviour
    {
        public Ring Data;
        public RingPiece RingCakePiecePrefab;
        public float GapWidthDegree = 1f;
        //public Action<string> callback;
        protected RingPiece[] Pieces;
        protected MenuController Parent;
        public string Path;

        void Start()
        {
            Display();
        }

        void Update()
        {
            
        }

        void Display()
        {
            var numButtons = Data.buttons.Length;
            var stepLength = 360f / numButtons;
            var iconDist = Vector3.Distance(RingCakePiecePrefab.Icon.transform.position, RingCakePiecePrefab.CakePiece.transform.position);

            //Position it
            Pieces = new RingPiece[numButtons];

            for (int i = 0; i < numButtons; i++)
            {
                Pieces[i] = Instantiate(RingCakePiecePrefab, transform);
                // Set root element
                Pieces[i].transform.localPosition = Vector3.zero;
                Pieces[i].transform.localRotation = Quaternion.identity;

                // Set piece position
                Pieces[i].CakePiece.fillAmount = 1f / numButtons - GapWidthDegree / 360f;
                Pieces[i].CakePiece.transform.localPosition = Vector3.zero;
                Pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f + GapWidthDegree / 2f + i * stepLength);
                Pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

                // Set icon position
                Pieces[i].Icon.transform.localPosition = Pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;
                Pieces[i].Icon.sprite = Data.buttons[i].Icon;

            }
        }

    }
}
