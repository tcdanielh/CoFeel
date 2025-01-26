using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FireBall : NetworkBehaviour
{
    public GameObject FireCracker;
    
    
    [ServerRpc(RequireOwnership = false)]
    private void InstantiateBoomServerRpc()
    {
        
        GameObject fireCracker = Instantiate(FireCracker, transform.position, transform.rotation);
        fireCracker.GetComponent<NetworkObject>().Spawn();
        Destroy(transform.gameObject);
    
    }
    private void InstantiateBoom()
    {
        
        GameObject fireCracker = Instantiate(FireCracker, transform.position, transform.rotation);
        fireCracker.GetComponent<NetworkObject>().Spawn();
        Destroy(transform.gameObject);
    
    }

    public void FireBoomSpawn()
    {
        
        if (!IsServer)
        {
            
            InstantiateBoomServerRpc();
        }
        else
        {
            
            InstantiateBoom();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "hittable" || other.gameObject.name.EndsWith("EffectMesh"))
        {
            FireBoomSpawn();
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "hittable" || other.gameObject.name.EndsWith("EffectMesh"))
        {
            FireBoomSpawn();
        }
    }
}
