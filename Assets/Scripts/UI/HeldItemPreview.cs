using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeldItemPreview : MonoBehaviour
    {
        private RawImage _rawImage;
        [SerializeField] private Texture2D held;
        [SerializeField] private Texture2D empty;
        // Start is called before the first frame update
        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _rawImage.texture = empty;
        }

        private void OnEnable()
        {
            PlayerController.OnSlotChanged += ChangeVisual;
        }

        private void OnDisable()
        {
            PlayerController.OnSlotChanged -= ChangeVisual;
        }

        private void ChangeVisual(GameObject newHeld)
        {
            _rawImage.texture = newHeld ? held : empty;
        }
    }
}
