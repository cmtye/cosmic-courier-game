using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility_Scripts
{
    public class InteractableHighlight : MonoBehaviour
    {
        [SerializeField] private Color color = Color.white;

        private List<Material> _materials;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        //Gets all the materials from each renderer
        private void Awake()
        {
            _materials = new List<Material>();
            foreach (var r in GetComponents<Renderer>())
            {
                _materials.AddRange(new List<Material>(r.materials));
            }
        }

        public void ToggleHighlight(bool highlight)
        {
            if (highlight)
            {
                foreach (var material in _materials)
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor(EmissionColor, color);
                }
            }
            else
            {
                foreach (var material in _materials)
                {
                    material.DisableKeyword("_EMISSION");
                }
            }
        }
    }

}
