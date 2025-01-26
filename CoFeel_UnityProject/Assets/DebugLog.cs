using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    public TextMeshPro text;

    public void Write(string s)
    {
        text.text += "\n" + s;
    }
}
