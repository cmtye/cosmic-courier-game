using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UX
{
    public class DepositBehavior : MonoBehaviour
    {
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private MaterialPropertyBlock _materialBlock;
        private MeshRenderer _meshRenderer;
        [SerializeField] private float currentValue = 0f;
        [SerializeField] private float maxValue = 10f;
        private bool _isShaking;

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
            var damagePercent = currentValue / maxValue;
            if (Math.Abs(damagePercent) > 0.001f) StartCoroutine(Shake(0.25f, 0.1f));
            _materialBlock.SetFloat(Fill, 1 - damagePercent);
            _meshRenderer.SetPropertyBlock(_materialBlock);
        }
        
        private IEnumerator Shake(float duration, float magnitude)
        {
            if (_isShaking) yield break;
            _isShaking = true;

            var originalPosition = transform.position;
            var elapsed = 0.0f;
            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * magnitude + originalPosition.x;
                var y = Random.Range(-1f, 1f) * magnitude + originalPosition.y;

                transform.position = new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return 0;
            }

            transform.position = originalPosition;
            _isShaking = false;
        }
    }
}
