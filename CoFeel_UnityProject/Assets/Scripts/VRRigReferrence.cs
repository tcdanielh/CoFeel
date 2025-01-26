using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferrence : MonoBehaviour
{
    public static VRRigReferrence Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Awake()
    {
        Singleton = this;
    }
}