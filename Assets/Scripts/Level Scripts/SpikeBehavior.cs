using System.Collections;
using UnityEngine;

namespace Level_Scripts
{
    public class SpikeBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject colliderObject;
        private BoxCollider _collider;
        private Animator _animator;

        private WaitForSeconds _windUpWait;
        private WaitForSeconds _windDownWait;
        private WaitForSeconds _activeWait;
        private bool _readyToInitiate;
        private static readonly int Retract = Animator.StringToHash("Retract");
        private static readonly int Initiate = Animator.StringToHash("Initiate");

        [SerializeField] private AudioClip noise;

        // Start is called before the first frame update
        private void Start()
        {
            _readyToInitiate = true;
            _collider = colliderObject.GetComponent<BoxCollider>();
            _animator = GetComponent<Animator>();
            _collider.enabled = false;

            _windUpWait = new WaitForSeconds(2f);
            _activeWait = new WaitForSeconds(1);
            _windDownWait = new WaitForSeconds(0.5f);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_readyToInitiate) return;
            
            StartCoroutine(SpikeRoutine());
            _readyToInitiate = false;
        }

        private IEnumerator SpikeRoutine()
        {
            yield return _windUpWait;
            _animator.ResetTrigger(Retract);
            _animator.SetTrigger(Initiate);
            _collider.enabled = true;

            AudioManager.Instance.PlaySound(noise, .05f);
            
            yield return _activeWait;
            
            _animator.ResetTrigger(Initiate);
            _animator.SetTrigger(Retract);
            yield return _windDownWait;
            
            _collider.enabled = false;
            _readyToInitiate = true;
        }
    }
}
