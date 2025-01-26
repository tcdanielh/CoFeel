using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OVRManager : NetworkBehaviour
{
    public Transform chestPos;
    public MovementRecognizer move;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            move.FireBallSpawn();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            move.LightSpawn();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            move.WallSpawn();
        }
        
    }
}