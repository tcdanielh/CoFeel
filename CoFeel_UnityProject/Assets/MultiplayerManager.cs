using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private string serverAddress = "192.168.4.2"; // Replace with your local server IP
    [SerializeField] private ushort port = 7777;

    void Start()
    {
        if (Application.isEditor)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Server started in Editor.");
    }

    public void StartClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(serverAddress, port);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started on VR headset.");
    }
}
