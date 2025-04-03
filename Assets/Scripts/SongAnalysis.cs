using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SongAnalysis : MonoBehaviour
{
    public static float bassThreshold = 0.01f;
    public static float snareThreshold = 0.1f;
    public static float hihatThreshold = 0.1f;

    static public bool DetectBassBeat(float[] spectrum){
        float bassEnergy = 0;
        for (int i = 2; i < 20; i++) // Capture deeper bass up to ~500 Hz
        {
            bassEnergy += spectrum[i];
        }
        bassEnergy /= 18; // Average out the energy

        Debug.Log(bassEnergy);
        return bassEnergy > bassThreshold; // Trigger if energy is above threshold
    }
    
    static public bool DetectSnare(float[] spectrum){
        float snareEnergy = 0;
        for (int i = 20; i < 50; i++) // Check the 20th to 50th frequency bins
        {
            snareEnergy += spectrum[i]; 
        }
        snareEnergy /= 30; // Average out the energy

    return snareEnergy > snareThreshold; // Trigger if energy is above threshold
    }

    static public bool DetectHihat(float[] spectrum){
        float hihatEnergy = 0;
        for (int i = 100; i < 300; i++) // Check the 100th to 300th frequency bins
        {
            hihatEnergy += spectrum[i]; 
        }
        hihatEnergy /= 200; // Average out the energy

    return hihatEnergy > hihatThreshold; // Trigger if energy is above threshold
    }

}
