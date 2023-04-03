using System;
using Enemy_Scripts;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : Singleton<GameManager>
{

    private int _storedAmount;

    private int _depotAmount;
    [SerializeField] private int depotGoal = 0;

    private int _patienceAmount;
    [SerializeField] private int patienceMax;
    
    [SerializeField] private StorageBehavior storageDisplay;
    [SerializeField] private DepositBehavior depositBar;
    [SerializeField] private PatienceBehavior patienceBar;
    [SerializeField] private GameObject gamePausedCanvas;
    [SerializeField] private GameObject gameLostCanvas;
    [SerializeField] private GameObject gameWonCanvas;

    private void Start()
    {
        depositBar.SetMax(depotGoal);
     
        patienceBar.SetMax(patienceMax);
        _patienceAmount = patienceMax;
        patienceBar.SetCurrent(_patienceAmount);
    }

    public void Deposit(int value)
    {
        _depotAmount += value;
        depositBar.SetCurrent(_depotAmount);
        CheckStageStatus();
    }

    public void Store(int value)
    {
        _storedAmount += value;
        storageDisplay.SetCurrent(_storedAmount);
    }

    public bool Spend(int value)
    {
        if (value > _storedAmount) return false;

        _storedAmount -= value;
        storageDisplay.SetCurrent(_storedAmount);
        return true;
    }

    private void LosePatience(int value)
    {
        _patienceAmount -= value;
        patienceBar.SetCurrent(_patienceAmount);
        CheckStageStatus();
    }

    private void CheckStageStatus()
    {
        if (_depotAmount >= depotGoal)
        {
            Debug.Log("STAGE COMPLETE");
            //gamePausedCanvas.SetActive(false);
            gameLostCanvas.SetActive(false);
            gameWonCanvas.SetActive(true);
            ToggleFreeze();
            return;
        }

        if (_patienceAmount <= 0)
        {
            Debug.Log("LOST STAGE");
            //gamePausedCanvas.SetActive(false);
            gameWonCanvas.SetActive(false);
            gameLostCanvas.SetActive(true);
            ToggleFreeze();
        }
    }

    public void ToggleFreeze()
    {
        if (Time.timeScale != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnEnable()
    {
        EnemyBehavior.OnEndReached += LosePatience;
    }

    private void OnDisable()
    {
        EnemyBehavior.OnEndReached -= LosePatience;
    }
}
