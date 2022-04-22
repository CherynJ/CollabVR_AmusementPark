using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class CF_NetworkGrab : XRGrabInteractable, IPunOwnershipCallbacks
{
    private PhotonView view;

    protected override void Awake()
    {
        base.Awake();
        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start() {
        view = GetComponent<PhotonView>();
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (PhotonNetwork.InRoom)
        {
            view.RequestOwnership();
            Debug.Log("Ownership Requested");
        }
        base.OnSelectEntered(args);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log("Ownership Request Received");

        if (targetView.gameObject != this.gameObject) {
            return;
        }

        if (!IsSelectedBySocket() && targetView.Owner != requestingPlayer)
        {
            targetView.TransferOwnership(requestingPlayer);
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("Ownership Request Transfered");
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        
    }

    private bool IsSelectedBySocket()
    {
        if (isSelected && firstInteractorSelecting.transform.TryGetComponent(out XRSocketInteractor socket))
        {
            if (interactorsSelecting.Count > 1) {
                return false;
            }
            return true;
        }
        return false;
    }
}