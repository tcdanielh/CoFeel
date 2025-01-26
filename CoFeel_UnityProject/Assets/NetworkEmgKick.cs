using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEmgKick : MonoBehaviour
{
    private BrainWaveSimulator brainWaveSimulator;

    void Start()
    {
        brainWaveSimulator = GetComponent<BrainWaveSimulator>();
    }

    public void UpdateParticleSystemOnClients(float value)
    {
        Debug.Log("in here 1");
        if (IsServer)
        {
            Debug.Log("in here 2");
            UpdateParticleSystemClientRpc(value);
        }
    }

    [ClientRpc]
    void UpdateParticleSystemClientRpc(float value)
    {
        Debug.Log("Updating Particle System on Clients");
        brainWaveSimulator.UpdateParticleSystem(value);
    }
}
