using System.Collections;
using TMPro;
using UnityEngine;

namespace Level_Scripts
{
    public class KillzoneBehavior : MonoBehaviour
    {
        private PlayerController _lastDroppedPlayer;
        private readonly WaitForSeconds _cacheWaitSecond = new (1);
        
        [SerializeField] private GameObject respawnUI;
        [SerializeField] private TextMeshProUGUI respawnText;
        [SerializeField] private int respawnTimer;
        private bool _isShaking;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _lastDroppedPlayer = other.gameObject.GetComponent<PlayerController>();
                if (_lastDroppedPlayer.currentlyHeld)
                {
                    if (_lastDroppedPlayer.currentlyHeld.CompareTag("Item")) _lastDroppedPlayer.DropItemOnDeath();
                }
                StartCoroutine(RespawnSequence());
            }
            else if (other.CompareTag("Item"))
            {
                if (other.transform.parent)
                {
                    if (other.transform.parent.CompareTag("Player"))
                    {
                        other.transform.parent.GetComponentInChildren<PlayerController>().ResetPlayerAnimator();
                    }
                }
                Destroy(other.gameObject);
            }
        }

        private IEnumerator RespawnSequence()
        {
            _lastDroppedPlayer.gameObject.SetActive(false);
            if (respawnUI) respawnUI.SetActive(true);

            for (var i = respawnTimer; i > 0; i--)
            {
                if (respawnText)
                {
                    respawnText.text = i + "";
                    StartCoroutine(Shake(0.25f, 5f, respawnText.gameObject));
                }

                yield return _cacheWaitSecond;
            }

            var respawn = _lastDroppedPlayer.respawnPoint;
            _lastDroppedPlayer.InvokeSlotChange(_lastDroppedPlayer.currentlyHeld);
            _lastDroppedPlayer.gameObject.transform.position = respawn.position;
            _lastDroppedPlayer.gameObject.transform.rotation = respawn.rotation;

            _lastDroppedPlayer.gameObject.SetActive(true);
            if (respawnUI) respawnUI.SetActive(false);
        }
        
        private IEnumerator Shake(float duration, float magnitude, GameObject target)
        {
            if (_isShaking) yield break;
            _isShaking = true;

            var originalPosition = target.transform.position;
            var elapsed = 0.0f;
            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * magnitude + originalPosition.x;
                var y = Random.Range(-1f, 1f) * magnitude + originalPosition.y;

                target.transform.position = new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return 0;
            }

            target.transform.position = originalPosition;
            _isShaking = false;
        }
    }
}
