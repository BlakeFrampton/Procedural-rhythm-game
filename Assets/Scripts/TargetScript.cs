using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TargetScript : MonoBehaviour
{
    public ParticleSystem TargetDestroyEffect;
    private Color32 TargetColour;
    public float TotalTargetLifeTime;
    private float TimePerShrink;
    private float TargetDestroyScale = 0.8f;
    private ScoreScript ScoreScript;
    private ComposerScript Composer;
    private SettingsPasserScript SettingsPasser;
    private float targetsPerSecond;

    [Range(0, 250)] public byte RingVisibility;
    // Start is called before the first frame update
    void Start(){

        SettingsPasser = FindObjectOfType(typeof(SettingsPasserScript)) as SettingsPasserScript;

        int TotalShrinks = (int)(math.log(TargetDestroyScale)/math.log(0.999));
        TimePerShrink = TotalTargetLifeTime / TotalShrinks;

        ScoreScript = FindObjectOfType(typeof(ScoreScript)) as ScoreScript;
        Composer = FindObjectOfType(typeof(ComposerScript)) as ComposerScript;

        //asign here because tps changes
        targetsPerSecond = Composer.GetTargetsPerSecond();
        int r = 0;
        int g = 0;
        int b = 0;
        int rndInt = UnityEngine.Random.Range(1,4);
        switch(rndInt){
            case 1:
            r = 255;
            break;
            case 2:
            g = 255;
            break;
            case 3:
            b = 255;
            break;
        }
        rndInt = UnityEngine.Random.Range(1,3);
        if (rndInt == 1){
            rndInt = UnityEngine.Random.Range(1,256);
            if (r ==0){
                r = rndInt;
            } else{
                g = rndInt;
            }
        } else {
            rndInt = UnityEngine.Random.Range(1,256);
            if (g == 0){
                g = rndInt;
            }else{
                b = rndInt;
            }
        }
        SpriteRenderer TargetSprite = GetComponent<SpriteRenderer>();
        TargetColour =  new Color32((byte)r, (byte)g, (byte)b, 255);
        TargetSprite.color = TargetColour;
        GameObject Parent = transform.parent.gameObject;
        GameObject Ring = Parent.transform.Find("Ring").gameObject;
        Ring.gameObject.GetComponent<SpriteRenderer>().color = new Color32(0,0,0, RingVisibility);
        InvokeRepeating("TargetShrink", TimePerShrink,TimePerShrink);
    }

    // void Update(){
    //     SpriteRenderer TargetSprite = GetComponent<SpriteRenderer>();
    //     int scaledScore = 200 + getCurrentScore() * 55 / ScoreScript.GetScorePerTarget();
    //     // Debug.Log(scaledScore);
    //     TargetColour.a = (byte)scaledScore;
    //     TargetSprite.color = TargetColour;
    // }

    void TargetShrink(){
        transform.localScale = new Vector3(transform.localScale.x * 0.999f, transform.localScale.y * 0.999f, transform.localScale.z);
        if (transform.localScale.x < TargetDestroyScale){
            Destroy(transform.parent.gameObject);
        }
    }
    void OnMouseOver(){
        if(Input.GetMouseButtonDown(0)){
            //Particle Logic
            ParticleSystem Particle = Instantiate(TargetDestroyEffect, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = Particle.main;
            TargetColour.a = 255;
            main.startColor = (Color)TargetColour;
            Destroy(Particle.gameObject, main.duration);

            //Destroy Target
            Destroy(transform.parent.gameObject);

            ScoreScript.UpdateScore(getCurrentScore());
        }
    }

    private int getCurrentScore(){
        //Score should be based on how close to the next target spawn you are
        //Next target spawn is crotchet/targetspercrotchet
        int ScoreToAdd;

        float OptimalTime = SettingsPasser.targetsOnScreen / targetsPerSecond;
        int NumOfShrinks = (int)(math.log(transform.localScale.x) / math.log(0.999));
        float LifeTime = NumOfShrinks * TimePerShrink;
        float Error= OptimalTime - LifeTime;
        float MaximumError;
        if (Error >= 0 ){
            MaximumError = OptimalTime;
        } else {
            MaximumError = TotalTargetLifeTime - OptimalTime;
        }

        ScoreToAdd = (int)(ScoreScript.ScorePerTarget - math.abs(Error) / MaximumError * ScoreScript.ScorePerTarget);
        Debug.Log(ScoreToAdd);
        return ScoreToAdd;
    }
}
