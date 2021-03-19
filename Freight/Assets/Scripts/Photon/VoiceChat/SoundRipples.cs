using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using System.Linq;
using Photon.Voice.Unity;
using FrostweepGames.Plugins.Native;

public class SoundRipples : MonoBehaviourPun
{
    public ParticleSystem particles;
    
    public float decibelsValue = 0f;

    private AudioClip _clip;
    private int _lastPosition = 0;
    private string _microphoneDevice;
    private readonly float _updateFrequency = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) return;

        _microphoneDevice = CustomMicrophone.devices[0];

        Recorder recorder = PhotonVoiceNetwork.Instance.PrimaryRecorder;

        _clip = recorder.AudioClip;

        InvokeRepeating(nameof(UpdateRipples), 0, _updateFrequency);
    }

    private void UpdateRipples()
    {
        if (!photonView.IsMine) return;

        int currentPosition = CustomMicrophone.GetPosition(_microphoneDevice);

        if(currentPosition != _lastPosition)
        {
            int length = (int) _clip.length * _clip.frequency;
            float[] data = new float[length];

            if (currentPosition > _lastPosition)
            {
                _clip.GetData(data, _lastPosition);
                int len = currentPosition - _lastPosition;
                decibelsValue = ComputeDB(data, 0, ref len);
                _lastPosition = currentPosition;
            } 
            else
            {
                _clip.GetData(data, _lastPosition);
                int len = data.Length - _lastPosition;
                decibelsValue = ComputeDB(data, 0, ref len);
                _lastPosition = 0;
            }
            //Debug.Log(decibelsValue);
            if (decibelsValue <= 0)
            {
                ParticleSystem.MainModule main = particles.main;
                main.startSize = 1;
            }
            else
            {
                ParticleSystem.MainModule main = particles.main;
                main.startSize = decibelsValue;
            }
        }

    }

    private float ComputeRMS(float[] buffer, int offset, ref int length)
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
            sos += val * val;
            offset++;
        }

        // return sqrt of average
        return Mathf.Sqrt(sos / length);
    }

    private float ComputeDB(float[] buffer, int offset, ref int length)
    {
        float rms;

        rms = ComputeRMS(buffer, offset, ref length);

        float refPower = 0.001f;
        return 10 * Mathf.Log10(rms / refPower);
    }
}
