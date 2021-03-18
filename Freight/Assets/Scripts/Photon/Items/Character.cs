﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun
{
    public Transform pickUpDestination;
    public Transform pickUpDestinationLocal;
    public Transform dragDestination;
    public PickUpable currentHeldItem;
    public GameObject bulletPrefab;
    
    public bool HasItem()
    {
        return currentHeldItem != null;
    }

    [PunRPC]
    void PickUpRPCLocal(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Debug.Log("LOCAL");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        Item.transform.position = pickUpDestinationLocal.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestinationLocal;
        Item.transform.Rotate(0, 90, 0);

        Item.SetItemPickupConditions();
    }

    [PunRPC]
    void PickUpRPC(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Debug.Log("drill");
        //PhotonView view = Item.GetComponent<PhotonView>();
        //view.TransferOwnership(PhotonNetwork.LocalPlayer);
        // Move to players pickup destination.
        Item.transform.position = pickUpDestination.position;

        // Set the parent of the object to the pickupDestination so that it moves
        // with the player.
        Item.transform.parent = pickUpDestination;
        Item.transform.Rotate(0, 90, 0);

        Item.SetItemPickupConditions();
    }

    public void PickUp(PickUpable Item) 
    {
        currentHeldItem = Item;

        PhotonView view = Item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        //Item.SetItemPickupConditions();

        if (Item.tag == "Gun")
        {
            Item.transform.GetChild(17).GetChild(0).gameObject.SetActive(true);
        }

        photonView.RPC("PickUpRPC", RpcTarget.Others, Item.transform.GetComponent<PhotonView>().ViewID);
        photonView.RPC("PickUpRPCLocal", PhotonNetwork.LocalPlayer, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void ThrowRPC(int ItemID)
    {
        Throwable Item = PhotonView.Find(ItemID).GetComponent<Throwable>();
        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;

        Item.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
        Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
    }

    public void Throw(Throwable Item) 
    {
        currentHeldItem = null;
        Item.ResetItemConditions(this);
        photonView.RPC("ThrowRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void DropRPC(int ItemID)
    {
        PickUpable Item = PhotonView.Find(ItemID).GetComponent<PickUpable>();
        Item.transform.Rotate(50, 50, 0);
        if (Item.GetComponent<Shootable>() != null)
        {
            Item.transform.parent = GameObject.Find("/Environment/Interactables/Guns").transform;
        }
        else
        {
            Item.transform.parent = GameObject.Find("/Environment/Interactables/DeadGuards").transform;
        }
        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 8f;
    }

    public void Drop(PickUpable Item) 
    {
        currentHeldItem = null;
        Item.ResetItemConditions(this);

        if (Item.tag == "Gun")
        {
            Item.transform.GetChild(17).GetChild(0).gameObject.SetActive(false);
        }

        //Item.transform.parent = GameObject.Find("/Environment/Interactables/Rocks").transform;
        photonView.RPC("DropRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void KillGuard(int guardId)
    {
        // get the guard's photon view
        PhotonView killedGuard = PhotonView.Find(guardId).GetComponent<PhotonView>();
        Vector3 guardPos = killedGuard.transform.position;
        // remove the guard 
        PhotonNetwork.Destroy(killedGuard);
        // create a dead body that will be draggable
        GameObject deadGuard = PhotonNetwork.Instantiate("PhotonPrefabs/DeadGuard", guardPos, Quaternion.Euler(90, 0, 0));
        
    }

    [PunRPC]
    void CreateBulletLocal()
    {
        //GameObject camera = pickUpDestination.transform.parent.gameObject;

        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;

        // shoots out a raycast to see what the bullet hits
        Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);

        // if bullet collides with guard, tell masterclient to kill guard
        if(hitInfo.collider != null)
            if(hitInfo.collider.GetComponent<GuardAIPhoton>() != null) {
                Debug.Log("Guard was hit acc");
                photonView.RPC("KillGuard", RpcTarget.MasterClient, hitInfo.collider.GetComponent<PhotonView>().ViewID);
            }
        // instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestinationLocal.transform.GetChild(0).transform.GetChild(14).position, pickUpDestinationLocal.transform.GetChild(0).rotation);
        pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().GunShot();

        // if it hits something, have the bullet point at that thing and add a force based on bullet forward facing transform
        // this is so the bullet goes towards crosshair
        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 1400);
        }
        // in case it doesn't hit anything, just add force based on camera transform
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1400);
        }
        // so bullet moves
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    [PunRPC]
    void CreateBullet()
    {
        //GameObject camera = pickUpDestination.transform.parent.gameObject;

        GameObject parent = pickUpDestination.transform.parent.gameObject;

        GameObject cube = parent.transform.GetChild(2).gameObject;

        GameObject camera = cube.transform.GetChild(0).gameObject;

        // shoots out a raycast to see what the bullet hits
        Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo);

        // instantiate the bullet locally
        GameObject bullet = Instantiate(bulletPrefab, pickUpDestination.transform.GetChild(0).transform.GetChild(14).position, pickUpDestination.transform.GetChild(0).rotation);
        //pickUpDestination.transform.GetChild(0).GetComponent<Gun>().GunShot();

        // if it hits something, have the bullet point at that thing and add a force based on bullet forward facing transform
        // this is so the bullet goes towards crosshair
        if (hitInfo.point != new Vector3(0f, 0f, 0f))
        {
            bullet.transform.LookAt(hitInfo.point);

            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 1400);
        }
        // in case it doesn't hit anything, just add force based on camera transform
        else
        {
            bullet.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1400);
        }
        // so bullet moves
        bullet.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Shoot(Shootable Item) 
    {
        // send an RPC to shoot a bullet on the local client and on all other clients
        // this is done because the local player has to hold the gun in a game object that is the child of the camera
        if (pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().Ammo > 0)
        {
            photonView.RPC("CreateBullet", RpcTarget.Others);
            photonView.RPC("CreateBulletLocal", PhotonNetwork.LocalPlayer);
        } 
        else
        {
            pickUpDestinationLocal.transform.GetChild(0).GetComponent<Gun>().EmptyGunShot();
        }
        
    }

    [PunRPC]
    void DragRPC(int ItemID)
    {
        Draggable Item = PhotonView.Find(ItemID).GetComponent<Draggable>();
        Item.transform.position = dragDestination.position;
        Item.transform.parent = dragDestination;

        Item.SetItemPickupConditions();
        Item.transform.Rotate(90, 0, 0);
        gameObject.transform.GetComponent<PlayerMovementPhoton>().Speed = 6f;
    }

    public void Drag(Draggable Item)
    {
        currentHeldItem = Item;

        PhotonView view = Item.GetComponent<PhotonView>();
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        photonView.RPC("DragRPC", RpcTarget.All, Item.transform.GetComponent<PhotonView>().ViewID);
    }

}
