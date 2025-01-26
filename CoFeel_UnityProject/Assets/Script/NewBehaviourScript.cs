using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class BrainWaveSimulator : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;

    public ParticleSystem particleSystem;
    public TextAsset eegDataFile;
    public float updateInterval = 5;
    public int windowSize = 250; // Number of samples to average

    // public AudioSource audioSource;
    public AudioClip calmClip;
    public AudioClip alertClip;
    
    private ParticleSystem.MainModule mainModule;
    private List<float> alphaBandData = new List<float>();
    private Queue<float> recentValues = new Queue<float>();
    private int currentIndex = 0;
    private float lastParticleValue;
    private float timer = 0f;

    private float sampleRate = 250f; // 250 Hz
    private float sampleInterval;
    private float sampleTimer = 0f;

    float minAlpha;
    float maxAlpha;

    // MUSIC STUFF
    public AudioMixer audioMixer;

    private string currentState = "";



    // load in the EEGData
    void LoadAlphaBandData()
    {
        if (eegDataFile == null) return;
        
        string[] lines = eegDataFile.text.Split('\n');
        bool headerFound = false;
        int alphaColumnIndex = -1;
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;


        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            
            string[] values = line.Trim().Split(',');
            if (!headerFound)
            {
                // Find AverageBandPowerAlpha column
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Trim() == "AverageBandPowerAlpha")
                    {
                        alphaColumnIndex = i;
                        break;
                    }
                }
                headerFound = true;
                continue;
            }

            if (alphaColumnIndex != -1 && values.Length > alphaColumnIndex)
            {
                if (float.TryParse(values[alphaColumnIndex], out float value))
                {
                    alphaBandData.Add(value);
                    minValue = Mathf.Min(minValue, value);
                    maxValue = Mathf.Max(maxValue, value);
                }
            }
        }
        minAlpha = minValue;
        maxAlpha = maxValue;
        Debug.Log($"Alpha power range: {minAlpha} to {maxAlpha}");
    }


    void Start()
    {
        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
        }
        
        LoadAlphaBandData();
        lastParticleValue = 0f;
        sampleInterval = 1f / sampleRate;
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
        if (alphaBandData.Count == 0) return;

        sampleTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // loop logic
        while (sampleTimer >= sampleInterval)
        {
            sampleTimer -= sampleInterval;
            currentIndex = (currentIndex + 1) % alphaBandData.Count;
            float currentValue = alphaBandData[currentIndex];
            
            recentValues.Enqueue(currentValue);
            if (recentValues.Count > windowSize)
            {
                recentValues.Dequeue();
            }
        }


        if (timer >= updateInterval)
        {
            timer = 0f;
            float averageValue = CalculateMovingAverage();
            // Log transform before normalizing
            float logValue = Mathf.Log(averageValue);
            float logMin = Mathf.Log(minAlpha);
            float logMax = Mathf.Log(maxAlpha);
            float normalizedValue = Mathf.InverseLerp(logMin, logMax, logValue);
            lastParticleValue = normalizedValue;
            
            Debug.Log($"Current Alpha Value: {alphaBandData[currentIndex]}, Normalized: {normalizedValue}");
            UpdateParticleSystem(lastParticleValue);
        }
    }

    private void UpdateParticleSystem(float value)
    {
        if (particleSystem == null) return;

        string newState;
        bool isCalm = false;

        if (value > 0.75f) { 
            mainModule.startSize = 2f;
            newState = "calm";
            isCalm = true;
        }
        else if (value > 0.5f) {
            mainModule.startSize = 5f;
            newState = "calm";
            isCalm = true;
        }
        else if (value > 0.25f) {
            mainModule.startSize = 25f;
            newState = "alert";
            isCalm = false;
        }
        else {
            mainModule.startSize = 50f;
            newState = "alert";
            isCalm = false;
        }

        Debug.Log(newState);

        // Only change audio if state has changed
        if (newState != currentState) {
            currentState = newState;
            audioSource.clip = isCalm ? calmClip : alertClip;
            audioSource.Play();
            Debug.Log($"Playing {(isCalm ? "Calm" : "Alert")} Audio");
        }
    }
}