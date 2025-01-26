using System;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public Renderer sphereRenderer;  // Assign the sphere's Renderer in the Inspector
    public Color leftColor = Color.blue;
    public Color rightColor = Color.red;

    private void Start()
    {
        // find object with tag "Ball" and assign the sphereRenderer
        sphereRenderer = GameObject.FindGameObjectWithTag("Ball").GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hand collider enters the left trigger
        if (other.CompareTag("LeftTrigger"))
        {
            // Debug.Log("Left Trigger");
            sphereRenderer.material.color = leftColor;
        }
        // Check if the hand collider enters the right trigger
        else if (other.CompareTag("RightTrigger"))
        {
            // Debug.Log("Right Trigger");
            sphereRenderer.material.color = rightColor;
        }
    }
}