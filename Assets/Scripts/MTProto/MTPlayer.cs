using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MTPlayer : MonoBehaviour
{
    public Camera HeadCamera;
    public GameObject HeadModel;
    public XRTestingActions XRInputs;

    private void OnEnable()
    {
        XRInputs.Enable();
        XRInputs.XRTestPlayer.Teleport.started += Teleport_Held;
        XRInputs.XRTestPlayer.Teleport.canceled += Teleport_Released;
    }

    private void Teleport_Held(InputAction.CallbackContext obj)
    {
        Debug.Log("Teleport held.");
    }

    private void Teleport_Released(InputAction.CallbackContext obj)
    {
        Debug.Log("Teleport released.");
    }


    private void OnDisable()
    {
        XRInputs.Disable();
    }

    void Awake()
    {
        SetupPlayer(true);
    }

    private void Update()
    {
    }

    void SetupPlayer(bool islocal)
    {
        XRInputs = new XRTestingActions();

        if (islocal) 
        {
            //Hide head locally
            foreach (MeshRenderer headpiece in HeadModel.GetComponentsInChildren<MeshRenderer>())
            {
                headpiece.enabled = false;
            }
        }
        else
        {
            //Disable remote player's trackers & Camera
            foreach (RBFollower rbfol in GetComponentsInChildren<RBFollower>())
            {
                rbfol.enabled = false;
            }
            HeadCamera.enabled = false;
        }
    }

}
