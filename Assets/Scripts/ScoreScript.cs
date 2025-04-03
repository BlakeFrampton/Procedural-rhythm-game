using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    private static int Score = 0;
    public static int ScorePerTarget;
    private TextMeshPro ScoreGainedText;
    // Start is called before the first frame update
    void Start()
    {
        ScoreGainedScript ScoreGainedScript = FindObjectOfType(typeof(ScoreGainedScript)) as ScoreGainedScript;
        ScoreGainedText = ScoreGainedScript.GetComponent<TextMeshPro>();
        ScorePerTarget = 50;

        var Text = gameObject.GetComponent<TextMeshPro>();
        Text.text = "0";
    }
    public void UpdateScore(int ScoreToAdd){
        Score += ScoreToAdd;
        var Text = gameObject.GetComponent<TextMeshPro>();
        Text.text = Score.ToString();

        Text.color = new Color32((byte)(1 - 255 * ScoreToAdd/ScorePerTarget), (byte)( 255 * ScoreToAdd/ScorePerTarget), 0, 255);

        ScoreGainedText.text = $"+{ScoreToAdd}";
        Color32 FontColour = ScoreGainedText.color;
        ScoreGainedText.color = new Color32(FontColour.r,FontColour.g,FontColour.b, 255);
    }
    
    public int GetScorePerTarget(){
        return ScorePerTarget;
    }
}
