using UnityEngine;
using System.IO;
using System;
using NAudio;
using NAudio.Wave;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager.UI;
using Unity.Mathematics;
using Unity.VisualScripting;

public static class NAudioPlayer
{
    public static AudioClip WavToAudioClip(byte[] data)
    {
        MemoryStream waveStream = new MemoryStream(data);
        WAV wav = new WAV(waveStream.ToArray());
		
        AudioClip audioClip;
        if (wav.ChannelCount == 2) {
            audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 2, wav.Frequency, false);
			 audioClip.SetData(wav.StereoChannel, 0);
        } else {
            audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 1, wav.Frequency, false);
			 audioClip.SetData(wav.LeftChannel, 0);
        }
        // Now return the clip
        return audioClip;
    }

    private static MemoryStream AudioMemStream(WaveStream waveStream)
    {
        MemoryStream outputStream = new MemoryStream();
        using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
        {
            byte[] bytes = new byte[waveStream.Length];
            waveStream.Position = 0;
            waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length)); 
            waveFileWriter.Write(bytes, 0, bytes.Length);
            waveFileWriter.Flush();
        }
        return outputStream;
    }
}
public class WAV{

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static int bytesToInt(byte[] bytes, int offset = 0)
    {
        int value = 0;
        for (int i = 0; i < 4; i++)
        {
            value |= ((int)bytes[offset + i]) << (i * 8);
        }
        return value;
    }
    // properties
    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }
	public float[] StereoChannel { get; internal set; }
    public int ChannelCount { get; internal set; }
    public int SampleCount { get; internal set; }
    public int Frequency { get; internal set; }

    public WAV(byte[] wav)
    {

        // Determine if mono or stereo
        ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        // Get the frequency
        Frequency = bytesToInt(wav, 24);

        // Get past all the other sub chunks to get to the data subchunk:
        int pos = 12;   // First Subchunk ID from 12 to 16

        // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
            pos += 4 + chunkSize;
        }
        pos += 8;

        // Pos is now positioned to start of actual sound data.
        SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[SampleCount];
        if (ChannelCount == 2) RightChannel = new float[SampleCount];
        else RightChannel = null;

        // Write to double array/s:
        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
            pos += 2;
            if (ChannelCount == 2)
            {
                RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
            }
            i++;
        }
		
		//Merge left and right channels for stereo sound
		if (ChannelCount == 2) {
			StereoChannel = new float[SampleCount * 2];
			//Current position in our left and right channels
			int channelPos = 0;
			//After we've changed two values for our Stereochannel, we want to increase our channelPos
			short posChange = 0;

			for (int index = 0; index < (SampleCount * 2); index++) {

				if (index % 2 == 0) {
					StereoChannel[index] = LeftChannel[channelPos];
					posChange++;
				} else {
					StereoChannel[index] = RightChannel[channelPos];
					posChange++;
				}
				//Two values have been changed, so update our channelPos
				if (posChange % 2 == 0) {
					if (channelPos < SampleCount) {
						channelPos++;
						//Reset the counter for next iterations
						posChange = 0;
					}
				}
			}
		} else {
			StereoChannel = null;
		}
        GetBPM(StereoChannel, SampleCount, Frequency);
    }
    private void GetBPM(float[] StereoChannel, int SampleCount, int Frequency){

        int SamplesPerBucket = Frequency / 100; //Gives buckets of 10ms
        int NumOfBuckets = SampleCount/(SamplesPerBucket * 2);
        float[,]Buckets = new float[NumOfBuckets + 1, SamplesPerBucket];
        int BucketNum = 0;
        int SampleNum = 0;
        Debug.Log($"samplesPerBucket: {SamplesPerBucket}");
        Debug.Log($"NumOfBuckets: {NumOfBuckets}");
        Debug.Log($"Total bucket samples: {SamplesPerBucket*NumOfBuckets}");
        Debug.Log($"Stereo Length {SampleCount}");
        for (int index = 0; index < SampleCount- 1; index++){
            if (index % SamplesPerBucket * 2== 0){
                BucketNum += 1;
                SampleNum = 0;
            }
            if (index % 2 == 0){
                SampleNum += 1;
            }
            //Debug.Log($"{BucketNum}  + {SampleNum}");
            if (BucketNum > NumOfBuckets -2 ){
                Debug.Log($"Bucket: {BucketNum}");
            }
            //float temp = Buckets[BucketNum, SampleNum];
            //Buckets[BucketNum, SampleNum] += (float)(((StereoChannel[index] / 65535) - 0.5) * 2); //normalisation formula: (x - x min)/(x max - x min) then -0.5 * 2 to get from 0 to 1, to -1 to 1
        }

        float[] PowerPerBucket = new float[NumOfBuckets];
        
        for (int bucket = 0; bucket < NumOfBuckets + 1; bucket++){
            float power = 0;
                for (int sample = 0; sample < SamplesPerBucket + 1; sample++){
                    power += math.pow(Buckets[bucket,sample], 2);
                }
            PowerPerBucket[bucket] = power/SamplesPerBucket;
        }

        float[] ChangeInPower = new float[NumOfBuckets];
        int ChangesAboveThreshold = 0;
        float BPMThreshold = 0.8f;
        for (int bucket = 0; bucket < NumOfBuckets; bucket++){
            ChangeInPower[bucket] = PowerPerBucket[bucket + 1] - PowerPerBucket[bucket];
            if (ChangeInPower[bucket] > BPMThreshold){
                ChangesAboveThreshold += 1;
            }
        }
        float MinutesOfAudio = NumOfBuckets / 100 / 60;
        int BPM = (int)(ChangesAboveThreshold / MinutesOfAudio);
        Debug.Log(BPM);
    }
}