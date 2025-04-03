using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ScoreGainedScript : MonoBehaviour
{
    public float TimeToFade;
    void Start()
    {
        InvokeRepeating("Fade", 0f, TimeToFade / 255);
    }

    void Fade()
    {
        //Reduce alpha to zero over time of TimeToFade
        Color32 FontColour = GetComponent<TextMeshPro>().color;
        GetComponent<TextMeshPro>().color = new Color32(FontColour.r,FontColour.g,FontColour.b, (byte)math.max(0, FontColour.a - 1));
    }
}
