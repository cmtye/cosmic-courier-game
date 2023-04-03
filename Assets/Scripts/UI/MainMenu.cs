using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject MainButtons;
    public GameObject LevelSelect;

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


    public void ShowSettings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }


}
