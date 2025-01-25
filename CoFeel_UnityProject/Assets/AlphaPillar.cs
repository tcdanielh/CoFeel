using UnityEngine;
using OpenBCI.Network.Streams; // make sure you import the right thing from the OpenBCI SDK package
using Sngty;
using System;

public class AlphaPillar : MonoBehaviour
{
    public GameObject pillar;  // make sure you have a GameObject in the scene to represent the pillar
    [SerializeField] private BandPowerStream Stream; // attach the AverageBandPowerStream from the Unity prefab
    private float pillarHeight;

    public SingularityManager singularityManager;

    // Start is called once before the first frame update
    void Start()
    {
        pillarHeight = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // log the alpha band power value to the unity console
        // Debug.Log("Test0: " + (float)Stream.Channels[0].Alpha);
        // Debug.Log("Test1: " + (float)Stream.Channels[1].Alpha);
        // Debug.Log(Stream.Channels[0].Alpha.GetType());

        // if (Stream.Channels[0] > 0.9)
        // {
        //     singularityManager.sendMessage("Kicked T");
        //     Debug.Log(Stream.Channels[0]);
        // }

        // scale the cylinder height based on the alpha band power
        pillarHeight = ((float)Math.Log(Stream.Channels[2].Alpha) - (float)Math.Log(Stream.Channels[3].Alpha));
        // pillarHeight = Stream.AverageBandPower.Alpha;
        
        Debug.Log("Pillar Height: " + pillarHeight);

        // // pillarHeight = Stream.Channels[0].Alpha;
        pillar.transform.localScale = new Vector3(1, (float)pillarHeight, 1);
    }
}