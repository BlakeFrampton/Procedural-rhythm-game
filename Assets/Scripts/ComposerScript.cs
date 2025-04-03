using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ComposerScript : MonoBehaviour
{
    public GameObject TargetPrefab;
    public float idealTargetsPerSecond = 4;
    private float BPM = 120;
    private float StartDelay = 1.0f;
    private float Crotchet;
    private float BaseTargetsPerCrotchet;
    private float CurrentTargetsPerCrotchet;
    private float TargetsPerSecond;
    public float BaseTargetSize;
    private float CurrentTargetSize;
    private float HorizontalExtension;
    private float VerticalExtentsion;
    private GameObject CurrentTarget;
    private SettingsPasserScript SettingsPasser;
    private AudioSource audioSource;
    public BassDetector bassDetector;
    public void LoadScene(string Game)
        {
        }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bassDetector = GetComponent<BassDetector>();
        SettingsPasser = FindObjectOfType(typeof(SettingsPasserScript)) as SettingsPasserScript;
        audioSource.clip = SettingsPasser.AudioFile;
        BPM = UniBpmAnalyzer.AnalyzeBpm(audioSource.clip);
        while (BPM < 100){
            BPM *= 2;
        }
        while (BPM > 200){
            BPM /= 2;
        }
        Debug.Log("BPM: " + BPM);
        Crotchet = 60 / BPM;
        //Targets per crotchet can be any int/2
        BaseTargetsPerCrotchet = Mathf.RoundToInt(2 * idealTargetsPerSecond * Crotchet) / 2.0f;
        CurrentTargetsPerCrotchet = BaseTargetsPerCrotchet;
        Debug.Log("Targets per crotchet: " + CurrentTargetsPerCrotchet);

        updateTargetsPerSecond();
        Debug.Log("Targets per second: " + TargetsPerSecond);

        HorizontalExtension = Camera.main.orthographicSize  * Screen.width / Screen.height;
        VerticalExtentsion = Camera.main.orthographicSize ;
        
        audioSource.clip.LoadAudioData();
        Debug.Log("AudioClip Length: " + audioSource.clip.length);

        CurrentTargetSize = BaseTargetSize;

        Invoke("StartSong", StartDelay);
        Invoke("SpawnTarget", StartDelay);
    }
    void Update(){
        updateTargetsPerCrotchet();
    }
    
    private void updateTargetsPerCrotchet(){
        if (bassDetector.isBass())
        {
            CurrentTargetsPerCrotchet = BaseTargetsPerCrotchet * 2f;
            CurrentTargetSize = BaseTargetSize + 0.5f;
        }
        else
        {
            CurrentTargetsPerCrotchet = BaseTargetsPerCrotchet;
            CurrentTargetSize = BaseTargetSize;
        }
        updateTargetsPerSecond();
    }

    private void updateTargetsPerSecond(){
        TargetsPerSecond = CurrentTargetsPerCrotchet * BPM / 60;
    }

    void StartSong(){
        audioSource.Play();
        Invoke("StopSong", audioSource.clip.length);
    }

    void StopSong(){
        audioSource.Stop();
    }

    void SpawnTarget(){
        if (audioSource.isPlaying) {
            GameObject Target = TargetPrefab.transform.Find("Target").gameObject;
            float TargetWidth = Target.GetComponent<SpriteRenderer>().bounds.size.x * CurrentTargetSize;
            float TargetHeight = Target.GetComponent<SpriteRenderer>().bounds.size.y * CurrentTargetSize;
            float x;
            float y;
            x = UnityEngine.Random.Range(-HorizontalExtension + TargetWidth /2, HorizontalExtension - TargetWidth/2);
            y = UnityEngine.Random.Range(-VerticalExtentsion + TargetHeight / 2, VerticalExtentsion - TargetHeight / 2);
            CurrentTarget = Instantiate(TargetPrefab, new Vector3(x,y,0), Quaternion.identity);
            CurrentTarget.transform.localScale = new Vector3(CurrentTargetSize, CurrentTargetSize, CurrentTargetSize);

            Invoke("SpawnTarget", 1/ TargetsPerSecond);
        }
    }

    public float GetTargetsPerSecond(){
        return TargetsPerSecond;
    }
}
