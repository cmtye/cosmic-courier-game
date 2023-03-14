using System.Collections;
using TMPro;
using UnityEngine;

namespace Utility
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private float timeBetweenUpdates;
        private TextMeshProUGUI _text;
        private WaitForSecondsRealtime _waitForFrequency;
 
        private void Start()
        {
            _waitForFrequency = new WaitForSecondsRealtime(timeBetweenUpdates);
            _text = GetComponent<TextMeshProUGUI>();
            StartCoroutine(FPS());
        }
    
        private IEnumerator FPS()
        {
            while (true)
            {
                // Capture frame-per-second
                var lastFrameCount = Time.frameCount;
                var lastTime = Time.realtimeSinceStartup;
                yield return _waitForFrequency;
                var timeSpan = Time.realtimeSinceStartup - lastTime;
                var frameCount = Time.frameCount - lastFrameCount;

                var fps = Mathf.RoundToInt(frameCount / timeSpan);
                _text.text = fps.ToString();
            
                _text.color = fps switch
                {
                    <= 30 => Color.red,
                    <= 60 => Color.yellow,
                    _ => Color.green
                };
            }
        }
    }
}
