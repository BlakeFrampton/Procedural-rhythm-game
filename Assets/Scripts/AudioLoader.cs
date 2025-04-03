using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AudioLoader : MonoBehaviour
{
    public string filePath = "file://C:/path/to/audio.wav";

    IEnumerator LoadAudio()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio Load Error: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                GetComponent<AudioSource>().clip = clip;
                GetComponent<AudioSource>().Play();
            }
        }
    }

}
