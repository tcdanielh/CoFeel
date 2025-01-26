using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenBCI.Network.Streams;

public class Outer : MonoBehaviour
{
    [SerializeField] private EMGStream Stream;
    public ParticleSystem particleSystem;
    // public int detectedValue = 0; 
    private bool isPlaying = false;

    void Update()
    {
        if (Stream.Channels[0] >= 0.8 || Stream.Channels[1] >= 0.8 && !isPlaying)
        {
            StartCoroutine(PlayParticleSystem());
        }
        else
        {
            isPlaying = false;
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
