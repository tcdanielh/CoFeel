using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveAFew : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Schedule children deactivation after 2 seconds
        Invoke("DeactivateChildren", 2f);

        // Schedule destruction of this GameObject after 5 seconds
        Destroy(gameObject, 5f);
    }

    // Method to deactivate all children
    void DeactivateChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}