// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using UnityEngine.UI;
// using System.Runtime;
// using System.Runtime.InteropServices;
// using System.Runtime.Serialization.Formatters.Binary;
// using System.Runtime.Serialization;
// using NAudio;
// using NAudio.Wave;
// using UnityEngine.Networking;
// using SimpleFileBrowser;
// using UnityEditor;
// using TMPro;

// public class UploadFileScript : MonoBehaviour{
//     private SettingsPasserScript SettingsPasser;
//     public Text pathText;
//     private float ScreenWidth;

//     private void Start(){
//         SettingsPasser = FindObjectOfType(typeof(SettingsPasserScript)) as SettingsPasserScript;
//         ScreenWidth = Camera.main.orthographicSize  * Screen.width / Screen.height;
//         Debug.Log(ScreenWidth);
//     } 
//     public void ReadWavSounds(){
//         string path = EditorUtility.OpenFilePanel("Select audio file","C:/Users/Blake/Desktop/Unity/Rhythm Game/Assets/Resources", "wav");
//         byte[] SoundFile = FileBrowserHelpers.ReadBytesFromFile(path);
//         SettingsPasser.AudioFile = NAudioPlayer.WavToAudioClip(SoundFile); 
//         string FileNameAndType = Path.GetFileName(path);
//         string FileName = "";
//         bool FileTypeReached= false;
//         foreach (char Character in FileNameAndType){
//             if (Character == "."[0]){
//                 FileTypeReached = true;
//             }
//             if (FileTypeReached == false){
//                 FileName += Character;
//             }
//         }
//         // TextMeshPro UploadButtonText;
//         // UploadButtonText = this.transform.GetChild(0).GetComponent<TextMeshPro>();
//         // UploadButtonText.text = FileName;
//         // UploadButtonText.fontSize = 1000;
//         // UploadButtonText.ForceMeshUpdate();
//         // while (UploadButtonText.bounds.size.x > 0.8 * ScreenWidth){
//         //     Debug.Log(UploadButtonText.bounds.size.x );
//         //     UploadButtonText.fontSize -= 1;
//         //     UploadButtonText.ForceMeshUpdate();
//         // }
//     }
// }
