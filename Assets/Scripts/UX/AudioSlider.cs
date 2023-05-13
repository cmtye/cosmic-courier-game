using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UX
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private bool musicSlider;

        private void Start()
        {
            if (musicSlider)
            {
                if (PlayerPrefs.HasKey("Music"))
                {
                    valueText.SetText(Mathf.RoundToInt(PlayerPrefs.GetFloat("Music") * 100) + "%");
                    GetComponent<Slider>().value = PlayerPrefs.GetFloat("Music");
                }
                else
                {
                    valueText.SetText(Mathf.RoundToInt(0.5f * 100) + "%");
                    GetComponent<Slider>().value = 0.5f;
                }
            }
            else
            {
                if (PlayerPrefs.HasKey("Effects"))
                {
                    valueText.SetText(Mathf.RoundToInt(PlayerPrefs.GetFloat("Effects") * 100) + "%");
                    GetComponent<Slider>().value = PlayerPrefs.GetFloat("Effects");
                }
                else
                {
                    valueText.SetText(Mathf.RoundToInt(0.5f * 100) + "%");
                    GetComponent<Slider>().value = 0.5f;
                }
            }
        }

        public void OnChangeSlider(float value)
        {
            valueText.SetText(Mathf.RoundToInt(value * 100) + "%");

            if (musicSlider)
            {
                mixer.SetFloat("Music", Mathf.Log10(value) * 20);
                PlayerPrefs.SetFloat("Music", value);
                PlayerPrefs.Save();
            }
            else
            {
                mixer.SetFloat("Effects", Mathf.Log10(value) * 20);
                PlayerPrefs.SetFloat("Effects", value);
                PlayerPrefs.Save();
            }
        }
}
}
