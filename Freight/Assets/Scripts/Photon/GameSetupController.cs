﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class GameSetupController : MonoBehaviour
{
    /*
     * All this script is doing in the end is creating the player object. The game is told to find this player object under a 
     * PhotonPrefabs folder and to look for the prefab named PhotonPlayer.
     * Everything is setting the starting position and rotation values.
     * */
     void Start()
    {
        CreatePlayer();
    }
    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",
                "PhotonPlayer"), new Vector3(255, 7, 252), Quaternion.identity);
    }
}
