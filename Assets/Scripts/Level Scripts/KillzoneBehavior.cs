using System.Collections;
using UnityEngine;

namespace Level_Scripts
{
    public class KillzoneBehavior : MonoBehaviour
    {
        private PlayerController _lastDroppedPlayer;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(RespawnSequence(other.gameObject));
            }
            else if (other.CompareTag("Item"))
            {
                Destroy(other.gameObject);
            }
        }

        private IEnumerator RespawnSequence(GameObject player)
        {
            yield return new WaitForSeconds(3);
            
            _lastDroppedPlayer = player.GetComponent<PlayerController>();
            var respawn = _lastDroppedPlayer.respawnPoint;
            _lastDroppedPlayer.InvokeSlotChange(_lastDroppedPlayer.currentlyHeld);
            player.transform.position = respawn.position;
            player.transform.rotation = respawn.rotation;

        }
    }
}
