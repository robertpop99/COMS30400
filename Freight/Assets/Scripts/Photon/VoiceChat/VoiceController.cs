using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;

public class VoiceController : MonoBehaviourPun
{

    public Recorder recorder;
    public AudioListener audioListener;
    void Start()
    {
        if (!photonView.IsMine) return;

        recorder.enabled = true;
        audioListener.enabled = true;
        recorder.InterestGroup = (byte) PhotonNetwork.CurrentRoom.GetHashCode();
    }
}
