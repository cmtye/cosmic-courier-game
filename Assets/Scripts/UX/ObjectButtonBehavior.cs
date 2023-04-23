using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UX
{
    public class ObjectButtonBehavior : 
        MonoBehaviour, 
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler {

        [SerializeField] private UnityEvent buttonEvent;
        [SerializeField] private bool toggleButton;
        [SerializeField] private Material toggleColor;
        
        [Range(0, 1)] [SerializeField] private float dimmedPercentage = .8f;
        [Range(0, 1)] [SerializeField] private float clickedPercentage = .6f;
        
        private Renderer[] _renderers;
        private List<Color> _colors;

        private bool _toggleClick;
        private bool _beingClicked;
        private bool _beingHovered;

        private void Start()
        {
            _colors = new List<Color>();
            _renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in _renderers)
            {
                _colors.Add(r.material.color);
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_beingClicked) return;
            
            var counter = 0;
            foreach (var c in _colors)
            {
                if (!_toggleClick && counter > 1)
                {
                    Color.RGBToHSV(toggleColor.color, out var h, out var s, out var v);
                    _renderers[counter].material.color = Color.HSVToRGB(h, s, v * clickedPercentage, false);
                }
                else
                {
                    Color.RGBToHSV(c, out var h, out var s, out var v);
                    _renderers[counter].material.color = Color.HSVToRGB(h, s, v * clickedPercentage, false);
                }
                counter++;

            }
            _beingClicked = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _beingClicked = false;
            if (toggleButton) _toggleClick = !_toggleClick;

            if (_beingHovered)
            {
                var counter = 0;
                foreach (var c in _colors)
                {
                    if (_toggleClick && counter > 1)
                    {
                        Color.RGBToHSV(toggleColor.color, out var h, out var s, out var v);
                        _renderers[counter].material.color = Color.HSVToRGB(h, s, v * dimmedPercentage, false);
                    }
                    else
                    {
                        Color.RGBToHSV(c, out var h, out var s, out var v);
                        _renderers[counter].material.color = Color.HSVToRGB(h, s, v * dimmedPercentage, false);
                    }
                    counter++;
                }
            }
            else
            {
                var counter = 0;
                foreach (var c in _colors)
                {
                    if (_toggleClick && counter > 1)
                    {
                        _renderers[counter].material.color = toggleColor.color;
                    }
                    else
                    {
                        _renderers[counter].material.color = c;
                    }
                    counter++;
                }
            }
            buttonEvent.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _beingHovered = true;
            
            var counter = 0;
            foreach (var c in _colors)
            {
                if (_toggleClick && counter > 1)
                {
                    Color.RGBToHSV(toggleColor.color, out var h, out var s, out var v);
                    _renderers[counter].material.color = Color.HSVToRGB(h, s, v * dimmedPercentage, false);
                }
                else
                {
                    Color.RGBToHSV(c, out var h, out var s, out var v);
                    _renderers[counter].material.color = Color.HSVToRGB(h, s, v * dimmedPercentage, false);
                }
                counter++;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _beingHovered = false;
            var counter = 0;
            foreach (var c in _colors)
            {
                if (_toggleClick && counter > 1)
                {
                    _renderers[counter].material.color = toggleColor.color;
                }
                else
                {
                    _renderers[counter].material.color = c;
                }
                counter++;
            }
        }

        public void SetInactive()
        {
            var counter = 0;
            foreach (var c in _colors)
            {
                _renderers[counter].material.color = c;
                counter++;
            }
            gameObject.SetActive(false);
        }
    }
}
