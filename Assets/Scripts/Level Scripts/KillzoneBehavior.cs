using System.Collections;
using UnityEngine;

namespace Level_Scripts
{
    public class KillzoneBehavior : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(RespawnSequence(other.gameObject));
            }
        }

        private IEnumerator RespawnSequence(GameObject player)
        {
            yield return new WaitForSeconds(3);
            
            var respawn = player.GetComponent<PlayerController>().respawnPoint;
            player.transform.position = respawn.position;
            player.transform.rotation = respawn.rotation;

        }
    }
}
