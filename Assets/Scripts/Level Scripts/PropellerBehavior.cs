using UnityEngine;

namespace Level_Scripts
{
    public class PropellerBehavior : MonoBehaviour
    {
        private ImpactReceiver _playerReceiver;
        private RaycastHit[] _results;

        private void Start()
        {
            _results = new RaycastHit[50];
        }

        private void Update()
        {
            var currTransform = transform;
            var currPosition = currTransform.position;
            currPosition.y += 0.5f;
            var fwd = (currTransform).TransformDirection(Vector3.left) * 5;

            Debug.DrawRay(currPosition, fwd, Color.green);
            var size = Physics.SphereCastNonAlloc(currPosition, 0.5f, fwd.normalized, _results, 4, (1 << 10));
            for (var i = 0; i < size - 1; i++)
            {
                var hit = _results[i];
                if (hit.collider.CompareTag("Item")) continue;
            
                if (_playerReceiver)
                {
                    _playerReceiver.AddImpact(fwd, 0.5f);
                    return;
                }
            
                var impulse = hit.collider.GetComponent<ImpactReceiver>();
                if (impulse)
                {
                    _playerReceiver = impulse;
                    _playerReceiver.AddImpact(fwd, 0.5f);
                }
                else
                {
                    impulse = hit.transform.parent.GetComponent<ImpactReceiver>();
                    if (impulse)
                    {
                        _playerReceiver = impulse;
                        _playerReceiver.AddImpact(fwd, 0.5f);
                    }
                }
            }
        }
    }
}
