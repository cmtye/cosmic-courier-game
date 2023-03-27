using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeldItemPreview : MonoBehaviour
    {
        private RawImage _rawImage;
        [SerializeField] private Texture2D empty;

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
            if (!newHeld) 
            {
                _rawImage.texture = empty;
                return;
            }
            
            var iconContainer = newHeld.GetComponent<IconUI>();
            _rawImage.texture = iconContainer.icon;
        }
    }
}
