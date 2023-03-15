using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustTrail;

    public void SetDustTrail(bool value)
    {
        if (value)
        {
            if (!dustTrail.isPlaying)
            {
                dustTrail.Play();
            }
        }
        else
        {
            dustTrail.Stop();
        }
    }
}
