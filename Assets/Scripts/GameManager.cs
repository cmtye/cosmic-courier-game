using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private int _depotAmount = 0;

    [SerializeField]
    private int _depotGoal = 0;

    private DepositBehavior _depositBehavior;

    private int _disapointementAmount;

    private void Start()
    {
        _depositBehavior = GameObject.Find("DepositBar").GetComponent<DepositBehavior>();
        _depositBehavior.SetMax(_depotGoal);
    }


    public void Deposit(int value)
    {
        _depotAmount += value;
        _depositBehavior.SetCurrent(_depotAmount);
        CheckForSuccess();
    }

    public void Disappoint(int value)
    {
        _disapointementAmount += value;
    }

    private void CheckForSuccess()
    {
        if (_depotAmount >= _depotGoal)
            Debug.Log("STAGE COMPLETE");
    }


}
