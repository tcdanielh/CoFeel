using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CubeSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject objectPrefab; // Prefab for the object to spawn

    // Called when the client or server is ready
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Check if this is the client
        if (IsClient)
        {
            // Request the server to spawn the object when the client connects
            RequestSpawnServerRpc(transform.position + transform.up * 0.7f + transform.forward * 0.2f); 
        }
    }

    // ServerRpc to request the server to spawn the object
    [ServerRpc(RequireOwnership = false)] 
    private void RequestSpawnServerRpc(Vector3 spawnPosition, ServerRpcParams rpcParams = default)
    {
        // The server handles spawning the object
        SpawnObject(spawnPosition);
    }

    private void SpawnObject(Vector3 position)
    {
        if (IsServer)
        {
            // Instantiate the object on the server
            GameObject spawnedObject = Instantiate(objectPrefab, position, Quaternion.identity);

            // Spawn the object for all clients
            NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
            networkObject.Spawn(); 
            
            // request ownership of the object
            networkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        }
    }
}