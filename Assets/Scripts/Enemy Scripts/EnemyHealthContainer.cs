using UnityEngine;

namespace Enemy_Scripts
{
    public class EnemyHealthContainer : MonoBehaviour
    {
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private MaterialPropertyBlock _materialBlock;
        private MeshRenderer _meshRenderer;
        private Camera _mainCamera;
        public EnemyHealthBehavior HealthBehavior { private get; set; }

        private void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialBlock = new MaterialPropertyBlock();
        }

        // Hold reference to main scene camera that we adjust for.
        private void Start() { _mainCamera = Camera.main; }

        private void Update() 
        {
            // Only display health bar when on partial health. Allows border as a child for visibility.
            if (HealthBehavior.CurrentHealth <= HealthBehavior.MaxHealth) {
                if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(true);
                if (HealthBehavior.CurrentHealth <= 0) Destroy(gameObject, 0.6f);
            
                _meshRenderer.enabled = true;
                AlignCamera();
                UpdateParams();
            } 
            else 
            {
                if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
                _meshRenderer.enabled = false;
            }
        }

        private void UpdateParams() 
        {
            _meshRenderer.GetPropertyBlock(_materialBlock);
            _materialBlock.SetFloat(Fill, 1 - HealthBehavior.CurrentHealth / HealthBehavior.MaxHealth);
            _meshRenderer.SetPropertyBlock(_materialBlock);
        }

        // Health bars rotate towards cameras location. Gives 3D view of health bar.
        private void AlignCamera()
        {
            if (!_mainCamera) return;
        
            var cameraTransform = _mainCamera.transform;
            var forward = transform.position - cameraTransform.position;
            forward.Normalize();
        
            var up = Vector3.Cross(forward, cameraTransform.right);
            
            var rotation = Quaternion.LookRotation(forward, up);
            rotation *= Quaternion.Euler(0, 0, 90);
            transform.rotation = rotation;
        }
    }
}
