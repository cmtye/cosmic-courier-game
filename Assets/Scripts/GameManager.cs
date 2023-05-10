using System;
using Enemy_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using UX;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private Vector3Int _stored;

    private int _deposited;
    [SerializeField] private int depotGoal = 0;

    private int _patienceAmount;
    [SerializeField] private int patienceMax;
    
    [SerializeField] private StorageBehavior[] storageDisplays;
    [SerializeField] private DepositBehavior depositBar;
    [SerializeField] private PatienceBehavior patienceBar;
    [SerializeField] private GameObject gamePausedCanvas;
    [SerializeField] private GameObject gameLostCanvas;
    [SerializeField] private GameObject gameWonCanvas;

    [SerializeField] private GameObject BGM;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip patienceLossBlip;

    private bool _isFrozen;

    private void Start()
    {
        depositBar.SetMax(depotGoal);
     
        patienceBar.SetMax(patienceMax);
        _patienceAmount = patienceMax;
        patienceBar.SetCurrent(_patienceAmount);
        UpdateStorageDisplays();
    }

    public void Deposit()
    {
        _deposited++;
        depositBar.SetCurrent(_deposited);
        if (CheckDepotFull())
        {
            Debug.Log("STAGE COMPLETE");
            gamePausedCanvas.SetActive(false);
            gameLostCanvas.SetActive(false);
            gameWonCanvas.SetActive(true);
            ToggleFreeze();
        }
    }
    
    public void Store(Item item)
    {
        switch(item)
        {
            case Item.Small:
                _stored += Vector3Int.right;
                break;
            case Item.Medium:
                _stored += Vector3Int.up;
                break;
            case Item.Large:
                _stored += Vector3Int.forward;
                break;
            default:
                Debug.Log("Shouldn't be here...");
                break;
        }
        Debug.LogFormat("The stored item amounts are {0}", _stored.ToString());
        UpdateStorageDisplays();
    }

    public bool Spend(Vector3Int cost)
    {
        if (cost.x > _stored.x ||
            cost.y > _stored.y ||
            cost.z > _stored.z) 
        {
            return false;
        }

        _stored -= cost;

        Debug.LogFormat("The stored item amounts are {0}", _stored.ToString());
        UpdateStorageDisplays();
        return true;
    }

    void UpdateStorageDisplays()
    {
        storageDisplays[0].SetCurrent(_stored.x);
        storageDisplays[1].SetCurrent(_stored.y);
        storageDisplays[2].SetCurrent(_stored.z);
    }

    private void LosePatience(int value)
    {
        AudioManager.Instance.PlaySound(patienceLossBlip, .1f);

        _patienceAmount -= value;
        patienceBar.SetCurrent(_patienceAmount);
        if (_patienceAmount <= 0)
        {
            Debug.Log("LOST STAGE");
            gamePausedCanvas.SetActive(false);
            gameWonCanvas.SetActive(false);
            gameLostCanvas.SetActive(true);
            ToggleFreeze();
        }
        
    }

    public void TogglePause()
    {
        if (!_isFrozen)
        {
            gameWonCanvas.SetActive(false);
            gameLostCanvas.SetActive(false);
            gamePausedCanvas.SetActive(true);
            BGM.GetComponent<AudioSource>().volume = BGM.GetComponent<AudioSource>().volume / 3;
            ToggleFreeze();
            return;
        }
        gameWonCanvas.SetActive(false);
        gameLostCanvas.SetActive(false);
        gamePausedCanvas.SetActive(false);
        BGM.GetComponent<AudioSource>().volume = BGM.GetComponent<AudioSource>().volume * 3;
        ToggleFreeze();
    }

    public bool CheckDepotFull()
    {
        return _deposited >= depotGoal;
    }
    
    public void ToggleFreeze()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            _isFrozen = true;
        }
        else
        {
            Time.timeScale = 1;
            _isFrozen = false;
        }
    }

    public void PlayButtonClick()
    {
        AudioManager.Instance.PlaySound(buttonClick, .1f);
    }
    
    public void LoadMainMenu()
    {
        PlayButtonClick();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        PlayButtonClick();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void ReloadCurrentScene()
    {
        PlayButtonClick();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
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
