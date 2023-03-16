using System.Collections;
using System.Collections.Generic;
using Enemy_Scripts;
using UnityEngine;

public class TowerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        var attack = Physics.OverlapSphere(transform.position, 2f, -1);
        foreach (var a in attack)
        {
            if (!a.transform.parent) return;
            if (a.transform.parent.CompareTag("Enemy"))
            {
                a.gameObject.GetComponentInParent<EnemyHealthBehavior>().DealDamage(1f, ElementalTypes.Standard);
            }
        }
    }
}
