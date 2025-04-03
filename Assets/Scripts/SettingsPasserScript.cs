using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPasserScript : MonoBehaviour
{
    public AudioClip AudioFile;
    public int targetsOnScreen = 1;
    void Awake() {
        DontDestroyOnLoad(this);
    }
       
}
