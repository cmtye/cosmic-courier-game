using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropRates", menuName = "EnemyDropRates")]
public class DropRates : ScriptableObject
{

    public void UpdateGrossRates()
    {
        SmallGrossRate = StardustSmallRate;
        MediumGrossRate = SmallGrossRate + StardustMediumRate;
        LargeGrossRate = MediumGrossRate + StardustLargeRate;
        DarkGrossRate = LargeGrossRate + DarkMatterRate;
    }


    public float StardustSmallRate;
    public GameObject StardustSmallObject;
    public float SmallGrossRate { get; private set; }

    public float StardustMediumRate;
    public GameObject StardustMediumObject;
    public float MediumGrossRate { get; private set; }

    public float StardustLargeRate;
    public GameObject StardustLargeObject;
    public float LargeGrossRate { get; private set; }


    public float DarkMatterRate;
    public GameObject DarkMatterObject;
    public float DarkGrossRate { get; private set; }
}
