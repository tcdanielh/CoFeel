using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkEmgKick : NetworkBehaviour
{
    private emgKick _emgKick;

    void Start()
    {
        _emgKick = GetComponent<emgKick>();
    }

    public void UpdateKickOnClients()
    {
        Debug.Log("in here 3");
        if (IsServer)
        {
            Debug.Log("in here 4");
            UpdateKickClientRpc();
        }
    }

    [ClientRpc]
    void UpdateKickClientRpc()
    {
        Debug.Log("Updating kick on Clients");
        _emgKick.Spike();
    }
}
