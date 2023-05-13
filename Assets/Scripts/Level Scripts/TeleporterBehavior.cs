using System;
using UnityEngine;

namespace Level_Scripts
{
    public class TeleporterBehavior : MonoBehaviour
    {
        [SerializeField] private TeleporterBehavior linkedTeleporter;
        [SerializeField] private float internalTimer = 2f;
        [HideInInspector] public float resetTime;
        [SerializeField] private AudioClip noise;
        
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
            
            AudioManager.Instance.PlaySound(noise, 0.20f);
            other.transform.position = linkedTeleporter.transform.position;
            internalTimer = resetTime;
            linkedTeleporter.internalTimer = resetTime;
        }
    }
}
