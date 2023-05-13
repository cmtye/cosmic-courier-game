using UnityEngine;

namespace UX
{
    public class MainMenu : MonoBehaviour
    {

        public GameObject MainButtons;
        public GameObject LevelSelect;
        [SerializeField] private AudioClip click;

        public void Awake()
        {
            LevelSelect.SetActive(false);
        }

        public void ShowLevelSelect()
        {
            MainButtons.SetActive(false);
            LevelSelect.SetActive(true);
        }

        public void BackToMainMenu()
        {
            MainButtons.SetActive(true);
            LevelSelect.SetActive(false);
        }

        public void PlayButtonClick()
        {
            AudioManager.Instance.PlaySound(click, .1f);
        }
        
        public void ShowSettings()
        {

        }

        public void Quit()
        {
            Application.Quit();
        }


    }
}
