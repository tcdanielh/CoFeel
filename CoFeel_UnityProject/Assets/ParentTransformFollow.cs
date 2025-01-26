using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTransformFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = this.transform.parent.transform.position;
    }
}
