using System;
using UnityEngine;

namespace Level_Scripts
{
    public class TeleporterBehavior : MonoBehaviour
    {
        [SerializeField] private TeleporterBehavior linkedTeleporter;
        [SerializeField] private float internalTimer = 2f;
        [HideInInspector] public float resetTime;
        
        private void Start()
        {
            resetTime = internalTimer;
        }

        private void Update()
        {
            if (internalTimer > 0)
            {
                internalTimer -= Time.deltaTime;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!(internalTimer <= 0)) return;
            if (!other.CompareTag("Player")) return;
            
            other.transform.position = linkedTeleporter.transform.position;
            internalTimer = resetTime;
            linkedTeleporter.internalTimer = resetTime;
        }
    }
}
