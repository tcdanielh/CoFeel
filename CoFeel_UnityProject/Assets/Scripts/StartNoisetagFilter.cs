using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTagFlicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NoisetagController.Instance.startPrediction(50); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
