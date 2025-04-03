using System.Collections.Generic;
using UnityEngine;

public class BassDetector : MonoBehaviour
{
    private bool bass = false;
    public AudioSource audioSource;
    public int historySize = 25; // Roughly 1 second if running at 50 FPS
    public float bassThresholdMultiplier = 1.3f; // How much higher than average to trigger

    private Queue<float> bassHistory = new Queue<float>();

    public bool isBass(){
        return bass;
    }
    void Update()
    {
        float[] spectrum = new float[512];
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Compute bass energy over a range of bins
        float bassEnergy = ComputeBassEnergy(spectrum);

        // Store bass energy history
        if (bassHistory.Count >= historySize)
        {
            bassHistory.Dequeue(); // Remove oldest value
        }
        bassHistory.Enqueue(bassEnergy);

        // Compute average bass over history
        float avgBass = 0;
        foreach (float value in bassHistory)
        {
            avgBass += value;
        }
        avgBass /= bassHistory.Count;

        // Detect spike in bass
        if (bassEnergy > avgBass * bassThresholdMultiplier)
        {
            bass = true;
            Debug.Log("now bass");
        } else {
            bass = false;
            Debug.Log("no longer bass");
        }
    }

    float ComputeBassEnergy(float[] spectrum)
    {
        float bassEnergy = 0;
        for (int i = 2; i < 20; i++) // Focus on ~50-500 Hz
        {
            bassEnergy += spectrum[i] * (1.0f / (i + 1)); // Weigh lower frequencies more
        }
        return bassEnergy;
    }
}
