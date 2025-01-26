using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

using OpenBCI.Network.Streams;

public class BrainWaveSimulator : MonoBehaviour
{

    [SerializeField] private AverageBandPowerStream Stream;


    public ParticleSystem particleSystem;
    public float updateInterval = 5f;
    public int windowSize = 125; // Number of samples to average, experiment with this
    
    private ParticleSystem.MainModule mainModule;
    private Queue<float> recentValues = new Queue<float>();
    private float timer = 0f;

    private string currentState = "";



    void Start()
    {
        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
        }
    }

    float CalculateMovingAverage()
    {
        if (recentValues.Count == 0) return 0f;
        
        float sum = 0f;

        // sum recent values
        foreach (float value in recentValues)
        {
            sum += value;
        }
        return sum / recentValues.Count;
    }



    void Update()
    {
        timer += Time.deltaTime;

        float currentValue = Stream.AverageBandPower.Alpha;
        recentValues.Enqueue(currentValue);

        if (recentValues.Count > windowSize){
            recentValues.Dequeue();
        }

        if (timer >= updateInterval)
        {
            timer = 0f;
            float averageValue = CalculateMovingAverage();
            Debug.Log($"Current Average Alpha Value: {averageValue}");
            UpdateParticleSystem(averageValue);
        }
    }

    private void UpdateParticleSystem(float value)
    {
        if (particleSystem == null) return;

        string newState;

        if (value > 0.75f) { 
            mainModule.startSize = 2f;
            newState = "veryCalm";
        }
        else if (value > 0.5f) {
            mainModule.startSize = 5f;
            newState = "calm";
        }
        else if (value > 0.25f) {
            mainModule.startSize = 25f;
            newState = "alert";
        }
        else {
            mainModule.startSize = 50f;
            newState = "veryAlert";
        }

        if (newState != currentState)
        {
            currentState = newState;
            Debug.Log(currentState);
        }
    }
}
