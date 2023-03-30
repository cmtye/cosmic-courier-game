using System;
using Enemy_Scripts;
using UI;
using UnityEngine;
using Utility;

public class GameManager : Singleton<GameManager>
{

    private int _storedAmount = 0;

    private int _depotAmount = 0;
    [SerializeField] private int depotGoal = 0;

    private int _patienceAmount;
    [SerializeField] private int patienceMax;
    
    [SerializeField] private StorageBehavior storageDisplay;
    [SerializeField] private DepositBehavior depositBar;
    [SerializeField] private PatienceBehavior patienceBar;

    private void Start()
    {
        depositBar.SetMax(depotGoal);
     
        _patienceAmount = patienceMax;
        patienceBar.SetCurrent(_patienceAmount);
        patienceBar.SetMax(patienceMax);
    }

    public void Deposit(int value)
    {
        _depotAmount += value;
        depositBar.SetCurrent(_depotAmount);
        CheckForSuccess();
    }

    public void Store(int value)
    {
        _storedAmount += value;
        storageDisplay.SetCurrent(_storedAmount);
    }

    public bool Spend(int value)
    {
        if (value > _storedAmount) return false;

        _storedAmount += value;
        return true;
    }

    private void LosePatience(int value)
    {
        _patienceAmount -= value;
        patienceBar.SetCurrent(_patienceAmount);
        CheckForLoss();
    }

    private void CheckForSuccess()
    {
        if (_depotAmount >= depotGoal)
            Debug.Log("STAGE COMPLETE");
    }

    private void CheckForLoss()
    {
        if (_patienceAmount <= 0)
            Debug.Log("LOST STAGE");
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
