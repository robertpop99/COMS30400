using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class VoiceController : MonoBehaviourPun
{

    public AudioListener audioListener;
    void Start()
    {
        if (!photonView.IsMine) return;

        audioListener.enabled = true;
        byte newInterestGroup = (byte) PhotonNetwork.CurrentRoom.GetHashCode();
        PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = newInterestGroup;
    }
}
