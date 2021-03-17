using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;

public class VoiceController : MonoBehaviourPun
{

    public Recorder recorder;
    void Start()
    {
        //recorder.InterestGroup = (byte) PhotonNetwork.CurrentRoom.GetHashCode();
    }
}
