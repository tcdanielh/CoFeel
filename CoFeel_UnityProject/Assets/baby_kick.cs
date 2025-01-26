using UnityEngine;
using System.Collections;

using OpenBCI.Network.Streams;

public class emgKick : MonoBehaviour
{
    [SerializeField] private EMGStream Stream;
    public Renderer rend;

    private float timer = 0f;
    private const float DELAY_TIME = 5f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
    }

    // Toggle the Object's visibility each second.
    void Update()
    {

        NetworkEmgKick networkEmgKick = GetComponent<NetworkEmgKick>();
            
        // networkedBrainWaveSimulator.UpdateParticleSystemOnClients(averageValue);
            
        if (networkEmgKick != null)
        {
            networkEmgKick.UpdateKickOnClients();
        }
        Spike();
    }

    public void Spike()
    {
        // Debug.Log("EMG Reading " + Stream.Channels[8]);
        if (Stream.Channels[8] > 0.8 && rend.enabled == false)
        {
            rend.enabled = true;
            timer = DELAY_TIME;
        }
        else if (rend.enabled)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                rend.enabled = false;
            }
        }
    }
}