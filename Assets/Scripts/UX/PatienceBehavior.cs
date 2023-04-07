using UnityEngine;

namespace UX
{
    public class PatienceBehavior : MonoBehaviour
    {
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private MaterialPropertyBlock _materialBlock;
        private MeshRenderer _meshRenderer;
        [SerializeField] private float currentValue = 0f;
        [SerializeField] private float maxValue = 10f;

        private void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialBlock = new MaterialPropertyBlock();
        }

        private void Start()
        {
            UpdateParams();
        }

        public void SetMax(int value)
        {
            maxValue = (float)value;
        }

        public void SetCurrent(int value)
        {
            currentValue = (float)value;
            UpdateParams();
        }

        private void UpdateParams()
        {
            _meshRenderer.GetPropertyBlock(_materialBlock);
            _materialBlock.SetFloat(Fill, 1 - currentValue / maxValue);
            _meshRenderer.SetPropertyBlock(_materialBlock);
        }
    }
}
