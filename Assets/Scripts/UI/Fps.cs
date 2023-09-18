using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fps : MonoBehaviour
{
    public TextMeshProUGUI numberTMP;
    public void UpdateValue(float value)
    {
        numberTMP.text = value == 0 ? "FPS: VSunc on" : "FPS: " + value.ToString();
    }
}
