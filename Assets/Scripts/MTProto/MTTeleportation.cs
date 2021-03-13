using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTTeleportation : MonoBehaviour
{
    MTPlayer player;

    public float MaxTeleportDistance;

    public LayerMask ValidTeleportLayers;

    public AudioClip Clip_Click;
    public AudioClip Clip_Teleport;
    public AudioClip Clip_Snap;

    bool castingTeleport;

    Vector3 teleportCastPosition;
    RaycastHit teleportCastHit;

    public LineRenderer TeleportLine;

    public GameObject TeleportMarker;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<MTPlayer>();
        //Surround in an islocal when we get views going
        player.XRInputs.XRTestPlayer.Teleport.started += Teleport_started;
        player.XRInputs.XRTestPlayer.Teleport.canceled += Teleport_finish;
    }

    private void Teleport_finish(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        castingTeleport = false;
        TeleportMarker.SetActive(false);
    }

    private void Teleport_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        castingTeleport = true;
        TeleportMarker.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (castingTeleport)
        {
            //replace with arcing teleport line;
            TeleportLine.SetPosition(0, player.LeftHand.transform.position);

            //Teleport ray
            Physics.Raycast(player.LeftHand.transform.position, player.LeftHand.transform.forward, MaxTeleportDistance, ValidTeleportLayers, QueryTriggerInteraction.Ignore);

            TeleportLine.SetPosition(1, teleportCastHit.point);

        }
        
    }

    void DoTeleport(Vector3 teledest)
    {
        transform.position = teleportCastHit.point;
    }
}
