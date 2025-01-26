using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }


    private void Update()
    {
        if (IsOwner)
        {
            root.position = VRRigReferrence.Singleton.root.position;
            root.rotation = VRRigReferrence.Singleton.root.rotation;
            
            head.position = VRRigReferrence.Singleton.head.position;
            head.rotation = VRRigReferrence.Singleton.head.rotation;
            
            leftHand.position = VRRigReferrence.Singleton.leftHand.position;
            leftHand.rotation = VRRigReferrence.Singleton.leftHand.rotation;
            
            rightHand.position = VRRigReferrence.Singleton.rightHand.position;
            rightHand.rotation = VRRigReferrence.Singleton.rightHand.rotation;
            
        }
    }
}
