using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fracture : NetworkBehaviour
{
    public AudioSource soundEarth;

    private void Start()
    {
        foreach (Transform ch in this.transform)
        {
            Rigidbody rb = ch.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
            }
        }
        
    }

    //
    // [ServerRpc(RequireOwnership = false)]
    // private void InstantiateFracServerRpc()
    // {
    //     
    //     GameObject temp = Instantiate(fracturedObject, transform.position, transform.rotation);
    //     temp.GetComponent<NetworkObject>().Spawn();
    //     Destroy(transform.gameObject);
    //
    // }
    // private void InstantiateFrac()
    // {
    //     GameObject temp = Instantiate(fracturedObject, transform.position, transform.rotation);
    //     temp.GetComponent<NetworkObject>().Spawn();
    //     Destroy(transform.gameObject);
    //
    // }
    //
    // public void FracSpawn()
    // {
    //     if (!IsServer)
    //     {
    //         InstantiateFracServerRpc();
    //     }
    //     else
    //     {
    //         InstantiateFrac();
    //     }
    // }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("attack"))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            //ShatterObject();
            
            if (!IsServer)
            {
                DisappearCoroutineServerRpc();
            } else
            {
                DisappearCoroutineClientRpc();
            }
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("attack"))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            //ShatterObject();
            if (!IsServer)
            {
                DisappearCoroutineServerRpc();
            }
            else
            {
                DisappearCoroutineClientRpc();
            }
        }
    }
    
    void ShatterObject()
    {
        soundEarth.Play();
        // Apply forces to each piece of the fractured object
        foreach (Transform piece in this.transform)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.isKinematic = false;
                // Apply a random explosion force for a shattering effect
                Vector3 explosionDir = (piece.transform.position - this.transform.position).normalized;
                float explosionForce = Random.Range(10f, 50f);
                rb.AddForce(explosionDir * explosionForce);
                rb.isKinematic = false;
            }
        }
    }


  
    [ServerRpc(RequireOwnership = false)]
    void DisappearCoroutineServerRpc()
    {
        StartCoroutine(DisappearCoroutine());
    }

    [ClientRpc(RequireOwnership = false)]
    void DisappearCoroutineClientRpc()
    {
        StartCoroutine(DisappearCoroutine());
    }
    
    IEnumerator DisappearCoroutine()
    {
       
        Destroy(gameObject);
        yield return new WaitForSeconds(0.1f);
    }
}
