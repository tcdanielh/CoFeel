using Unity.Netcode;
using UnityEngine;

public class OVRManagerSpawner : NetworkBehaviour
{
    public GameObject ovrManagerPrefab;
    public MovementRecognizer move;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Instantiate the OVRManager prefab
            GameObject ovrManagerInstance = Instantiate(ovrManagerPrefab);
            ovrManagerInstance.GetComponent<OVRManager>().move = move;
            ovrManagerInstance.GetComponent<OVRManager>().chestPos = move.chestSource;

            // Get the NetworkObject component
            NetworkObject networkObject = ovrManagerInstance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                // Spawn the NetworkObject on the network
                networkObject.Spawn();

                // Optionally assign ownership to a client
                // networkObject.SpawnWithOwnership(clientId);
            }
            else
            {
                Debug.LogError("OVRManager prefab does not have a NetworkObject component.");
            }
        }
    }
}