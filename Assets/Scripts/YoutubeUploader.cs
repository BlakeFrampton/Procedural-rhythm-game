using System.IO;
using UnityEngine;
using NAudio.Wave;
using System.Diagnostics;
using UnityEngine.UI;
using TMPro;

public class YoutubeUploader : MonoBehaviour
{
    public TMP_InputField  urlInputField;  // Reference to the InputField for the URL
    public Button uploadButton;      // Reference to the Button
    public TMP_Text  statusText;           // Reference to the Text element for status updates (optional)

    private string saveDirectory;
    private string soundFile;
    private string youtubeUrl;
    private SettingsPasserScript SettingsPasser;
    private bool isButtonListenerAdded = false;
    void Start()
    {
        SettingsPasser = FindObjectOfType(typeof(SettingsPasserScript)) as SettingsPasserScript;
        saveDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "SavedAudio");
        soundFile = Path.Combine(saveDirectory, "audio.wav");


        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);
    }

    void Update()
    {
        // Check if the button is active in hierarchy and if the listener is not added yet
        if (uploadButton != null && uploadButton.gameObject.activeInHierarchy && !isButtonListenerAdded)
        {
            // Add the listener dynamically when the button becomes active
            uploadButton.onClick.AddListener(OnUploadButtonClicked);
            isButtonListenerAdded = true;  // Mark the listener as added
        }
    }

    void OnUploadButtonClicked()
    {
        UnityEngine.Debug.Log("on button click");
        youtubeUrl = urlInputField.text; // Get the URL from the InputField

        if (string.IsNullOrEmpty(youtubeUrl))
        {
            statusText.text = "Please enter a valid YouTube URL."; // Display error if URL is empty
            return;
        }

        statusText.text = "Starting conversion..."; // Update status text

        // Start the conversion process
        StartCoroutine(DownloadAndConvertYouTubeAudio(youtubeUrl));
    }

private System.Collections.IEnumerator DownloadAndConvertYouTubeAudio(string url)
{
    yield return new WaitForSeconds(1); // Prevents Unity freezing while running external processes

    string ffmpegPath = "C:/ProgramData/chocolatey/bin/ffmpeg.exe"; 
    string saveDirectory = "C:/Users/Blake/Desktop/SavedAudio"; // Ensure this directory exists
    string tempFile = Path.Combine(saveDirectory, "temp_downloaded_audio.opus"); // Ensure we handle the .opus extension
    string properWavFile = Path.Combine(saveDirectory, "audio_proper.wav"); // Corrected WAV file

    if (File.Exists(tempFile))
    {
        File.Delete(tempFile);
        UnityEngine.Debug.Log($"Deleted old file: {tempFile}");
    }
    if (File.Exists(properWavFile))
    {
        File.Delete(properWavFile);
        UnityEngine.Debug.Log($"Deleted old file: {properWavFile}");
    }

    // Download the audio (this can be in any format, but we'll get it as a raw file, not specifying .opus)
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "yt-dlp",
        Arguments = $"--ffmpeg-location \"{ffmpegPath}\" --extract-audio --audio-format opus --output \"{tempFile}\" {url}",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    Process process = new Process { StartInfo = psi };
    process.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log("STDOUT (yt-dlp): " + e.Data);
    process.ErrorDataReceived += (sender, e) => UnityEngine.Debug.LogError("STDERR (yt-dlp): " + e.Data);

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    process.WaitForExit();

    // Check if yt-dlp finished correctly
    if (process.ExitCode != 0)
    {
        UnityEngine.Debug.LogError("yt-dlp failed to execute successfully. Exit code: " + process.ExitCode);
        statusText.text = "Failed to download audio. Exit code: " + process.ExitCode;
        yield break; // Exit the coroutine early
    }

    // Check if the file exists after download
    if (File.Exists(tempFile))
    {
        UnityEngine.Debug.Log($"Downloaded file saved at: {tempFile}");
        FileInfo fileInfo = new FileInfo(tempFile);
        UnityEngine.Debug.Log($"File size: {fileInfo.Length} bytes");

        // Let's not delete the file yet and instead confirm it's correct
        UnityEngine.Debug.Log("Checking if file is a valid .opus file.");
        
        // Check if the downloaded .opus file has content
        ProcessStartInfo ffmpegCheckPsi = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = $"-i \"{tempFile}\"",  // Simply check the file info
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Log ffmpeg output to verify the file type
        Process ffmpegCheckProcess = new Process { StartInfo = ffmpegCheckPsi };
        ffmpegCheckProcess.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log("STDOUT (ffmpeg check): " + e.Data);
        ffmpegCheckProcess.ErrorDataReceived += (sender, e) => UnityEngine.Debug.LogError("STDERR (ffmpeg check): " + e.Data);

        ffmpegCheckProcess.Start();
        ffmpegCheckProcess.BeginOutputReadLine();
        ffmpegCheckProcess.BeginErrorReadLine();
        ffmpegCheckProcess.WaitForExit();

        // Proceed only if the file is a valid .opus file
        if (File.Exists(tempFile))
        {
            UnityEngine.Debug.Log($"File is valid: {tempFile}");
            
            // Perform conversion to .wav
            ProcessStartInfo ffmpegPsi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-i \"{tempFile}\" -vn -acodec pcm_s16le -ar 44100 -ac 2 \"{properWavFile}\"",  // Convert to .wav
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process ffmpegProcess = new Process { StartInfo = ffmpegPsi };
            ffmpegProcess.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log("STDOUT (ffmpeg): " + e.Data);
            ffmpegProcess.ErrorDataReceived += (sender, e) => UnityEngine.Debug.LogError("STDERR (ffmpeg): " + e.Data);

            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();
            ffmpegProcess.WaitForExit();

            // Log the conversion status
            if (File.Exists(properWavFile))
            {
                UnityEngine.Debug.Log($"File successfully converted to WAV: {properWavFile}");
                statusText.text = "Download & Conversion Complete. Loading Audio...";
                SettingsPasser.AudioFile = WavToAudioClip(properWavFile); // Load the proper WAV file into Unity
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to convert the file to WAV.");
                statusText.text = "Failed to convert the file to WAV.";
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Downloaded file is not a valid .opus file.");
            statusText.text = "Downloaded file is not a valid .opus file.";
        }
    }
    else
    {
        UnityEngine.Debug.LogError("Failed to download the file.");
        statusText.text = "Failed to download or convert the file.";
    }
}



public static AudioClip WavToAudioClip(string filePath)
{
    using (var reader = new WaveFileReader(filePath))
    {
        UnityEngine.Debug.Log($"Channels: {reader.WaveFormat.Channels}, SampleRate: {reader.WaveFormat.SampleRate}");

        int totalSamples = (int)(reader.Length / reader.WaveFormat.BlockAlign) * reader.WaveFormat.Channels;
        int samplesPerChannel = totalSamples / reader.WaveFormat.Channels;

        float[] audioData = new float[totalSamples];

        byte[] buffer = new byte[reader.WaveFormat.BlockAlign];
        int i = 0;

        while (reader.Read(buffer, 0, buffer.Length) > 0)
        {
            if (reader.WaveFormat.Channels == 1) // Mono audio
            {
                short sample = (short)(buffer[0] | (buffer[1] << 8)); // Assuming 16-bit PCM
                audioData[i++] = sample / 32768f; // Convert to Unity's float range
            }
            else if (reader.WaveFormat.Channels == 2) // Stereo audio
            {
                // Left channel
                short sampleLeft = (short)(buffer[0] | (buffer[1] << 8));
                // Right channel
                short sampleRight = (short)(buffer[2] | (buffer[3] << 8));

                // Interleave stereo channels
                audioData[i++] = sampleLeft / 32768f; // Left channel
                audioData[i++] = sampleRight / 32768f; // Right channel
            }

            if (i >= totalSamples)
                break;
        }

        // Create AudioClip with the correct sample rate and number of channels
        AudioClip clip = AudioClip.Create("YouTubeAudio", samplesPerChannel, reader.WaveFormat.Channels, reader.WaveFormat.SampleRate, false);
        clip.SetData(audioData, 0);

        return clip;
    }
}
}
