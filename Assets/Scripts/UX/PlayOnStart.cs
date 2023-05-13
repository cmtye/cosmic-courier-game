using System.Collections;
using UnityEngine;

namespace UX
{
    public class PlayOnStart : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(PlayAfterSeconds(0.1f));
        }

        private IEnumerator PlayAfterSeconds(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            GetComponent<AudioSource>().Play();
        }
    }
}
