using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform player;
    void Update(){ 
        transform.LookAt(Camera.main.transform);
    }
}
