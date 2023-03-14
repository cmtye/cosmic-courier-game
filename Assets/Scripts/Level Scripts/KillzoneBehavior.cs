using UnityEngine;

namespace Level_Scripts
{
    public class KillzoneBehavior : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player has been knocked out");
            }
        }
    }
}
