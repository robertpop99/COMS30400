﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerUI;
    public string gesture;


    // Start is called before the first frame update
    void Start()
    {
        // gets the gesture from PoseParser 
        gesture = PoseParser.GETGestureAsString();
        // if photon view is player's, sets their local UI to active
        if (photonView.IsMine)
        {
            transform.Find("UI 1").gameObject.SetActive(true);
        }
        // if UI is not the player's, disable it
        if (!photonView.IsMine && GetComponent<PlayerMovementPhoton>() != null)
        {
            Debug.Log(" DISABLE CONTROLER ");
            transform.Find("UI 1").gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // update gesture
        string tempGesture = PoseParser.GETGestureAsString();
        if (tempGesture != gesture)
        {
            gesture = tempGesture;
        }
        //Debug.Log(gesture);
    }

    // checks if the player is pressing P
    public bool IsPressingP()
    {
         return Input.GetKeyDown(KeyCode.P);
    }

}
