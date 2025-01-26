using System;
using Sngty;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public Renderer sphereRenderer;  // Assign the sphere's Renderer in the Inspector
    public Color leftColor = Color.blue;
    public Color rightColor = Color.red;
    public SingularityManager singularityManager;

    private void Start()
    {
        sphereRenderer = GameObject.FindGameObjectWithTag("Ball").GetComponent<Renderer>();
        singularityManager = GameObject.FindGameObjectWithTag("SingularityManager").GetComponent<SingularityManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hand collider enters the left trigger
        if (other.CompareTag("LeftTrigger"))
        {
            sphereRenderer.material.color = leftColor;
            Debug.Log("left hit");
            singularityManager.sendMessage("1");
        }
        // Check if the hand collider enters the right trigger
        else if (other.CompareTag("RightTrigger"))
        {
            sphereRenderer.material.color = rightColor;
            Debug.Log("right hit");
            singularityManager.sendMessage("2");
        }
    }
}