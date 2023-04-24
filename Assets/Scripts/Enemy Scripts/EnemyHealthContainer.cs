using UnityEngine;

namespace Enemy_Scripts
{
    public class EnemyHealthContainer : MonoBehaviour
    {
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private MaterialPropertyBlock _materialBlock;
        private MeshRenderer _meshRenderer;
        public EnemyHealthBehavior HealthBehavior { private get; set; }

        private void Awake() 
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialBlock = new MaterialPropertyBlock();
            
            if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
            _meshRenderer.enabled = false;
        }

        private void OnEnable()
        {
            EnemyHealthBehavior.OnEnemyHit += UpdateParams;
        }

        private void OnDisable()
        {
            EnemyHealthBehavior.OnEnemyHit -= UpdateParams;
        }

        private void UpdateParams(EnemyBehavior enemy)
        {
            // We can use the hit event to update the health parameters instead of Update to save cycles
            if (HealthBehavior != enemy.GetHealth()) return;
            
            if (HealthBehavior.CurrentHealth <= HealthBehavior.MaxHealth) {
                if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(true);
                if (HealthBehavior.CurrentHealth <= 0) Destroy(gameObject, 0.6f);
            
                _meshRenderer.enabled = true;
                
                _meshRenderer.GetPropertyBlock(_materialBlock);
                _materialBlock.SetFloat(Fill, 1 - HealthBehavior.CurrentHealth / HealthBehavior.MaxHealth);
                _meshRenderer.SetPropertyBlock(_materialBlock);
            } 
            else 
            {
                if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
                _meshRenderer.enabled = false;
            }
        }
    }
}
