using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outer : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public int detectedValue = 0; 
    private bool isPlaying = false;

    void Update()
    {
        if (detectedValue == 1 && !isPlaying)
        {
            StartCoroutine(PlayParticleSystem());
        }
    }

    private System.Collections.IEnumerator PlayParticleSystem()
    {
        isPlaying = true;
        particleSystem.Play();
        yield return new WaitForSeconds(1f);
        particleSystem.Stop();
        isPlaying = false;
    }
}
