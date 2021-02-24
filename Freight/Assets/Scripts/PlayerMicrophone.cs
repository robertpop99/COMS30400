using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMicrophone : NetworkBehaviour
{
    public AudioSource audioSource;
    public AudioListener audioListener;
    private AudioClip clip;

    private const int BufferSize = 16384;
    private float[] buffer = new float[BufferSize];
    private int pos = -1;

    public override void OnStartAuthority()
    {
        audioSource.enabled = false;
        audioListener.enabled = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        //Microphone.Init();
        //Microphone.QueryAudioInput();
        //audioSource.clip = AudioClip.Create("Microphone_input", Microphone.GetSampleSize(), 1, Microphone.GetSampleRate(), true, PCMReaderCallback);
        MicrophoneWebGL.Init(BufferSize, 1);

        string initResult;
        do
        {
            initResult = MicrophoneWebGL.PollInit();
            Debug.Log(initResult);
        } while (initResult == "pending");
        Debug.Log(initResult);

        MicrophoneWebGL.Start();

        var numBuffers = MicrophoneWebGL.GetNumBuffers();
        Debug.Log("recorded " + numBuffers + " buffers");
      
        clip = AudioClip.Create("microphone", BufferSize, 1, 44100, true,
            data =>
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    pos = (pos + 1) % BufferSize;
                    if (pos == 0)
                    {
                        var ok = MicrophoneWebGL.GetBuffer(buffer);
                        if (!ok)
                            Debug.LogError("not ok");
                    }
                    data[i] = buffer[pos];
                }
            });
#else
        //audioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);

#endif
    }

    int frame = 0;
    [Client]
    public void Update()
    {
        int length = (int) clip.length;

        if (frame >= length * 30)
        { 
            frame = 0;
            int samples = clip.samples;
            float[] buffer = new float[samples];

            clip.GetData(buffer, 0);

            //Debug.Log(length);
            //Debug.Log(samples_per_frame);

            Debug.Log(ComputeDB(buffer, 0, ref samples));
        }
        else
        {
            frame++;
        }

    }

    public static float ComputeRMS(float[] buffer, int offset, ref int length)
    {
        // sum of squares
        float sos = 0f;
        float val;

        if (offset + length > buffer.Length)
        {
            length = buffer.Length - offset;
        }

        for (int i = 0; i < length; i++)
        {
            val = buffer[offset];
            //if (val > -0.001f && val < 0.001f) continue;
            sos += val * val;
            offset++;
        }

        // return sqrt of average
        return Mathf.Sqrt(sos / length);
    }

    public static float ComputeDB(float[] buffer, int offset, ref int length)
    {
        float rms;

        rms = ComputeRMS(buffer, offset, ref length);

        // could divide rms by reference power, simplified version here with ref power of 1f.
        // will return negative values: 0db is the maximum.
        return 10 * Mathf.Log10(rms);
    }

    //#if UNITY_WEBGL// && !UNITY_EDITOR
    //    private void PCMReaderCallback(float[] data) {
    //        Microphone.GetDataArray(data, data.Length);
    //    }
    //#endif

}
