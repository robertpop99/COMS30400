using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using FrostweepGames.Plugins.Native;

public class VoiceController : MonoBehaviourPun
{

    public AudioListener audioListener;
    public AudioClip audioClip;

    void Start()
    {
        if (!photonView.IsMine) return;

        audioListener.enabled = true;
        byte newInterestGroup = (byte) PhotonNetwork.CurrentRoom.GetHashCode();
        PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = newInterestGroup;

        Recorder recorder = PhotonVoiceNetwork.Instance.PrimaryRecorder;
        
        CustomMicrophone.RequestMicrophonePermission();
        CustomMicrophone.RefreshMicrophoneDevices();

        audioClip = CustomMicrophone.Start(CustomMicrophone.devices[0], true, 1, (int)recorder.SamplingRate);

        recorder.AudioClip = audioClip;

        recorder.StartRecording();
    }
}
