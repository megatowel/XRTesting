using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megatowel.Multiplex;
using Megatowel.NetObject;
using Megatowel.Multiplex.Extensions;
using MTXR.Player;
using UnityEngine.InputSystem;

public class NetTest : NetBehaviour
{
    // Update is called once per frame, as you might know by now :3
    void Update()
    {
        if (Keyboard.current.cKey.isPressed)
        {
            netView.Submit(true);
            Debug.Log("we ownin");
        }
        if (netView.IsOwned)
        {
            netView.netObject.fields[1] = transform.position.ToBytes();
            netView.netObject.fields[2] = transform.rotation.ToBytes();
            netView.Submit();
        }
        else
        {
            transform.position = netView.netObject.submittedfields[1].FromBytes<Vector3>();
            transform.rotation = netView.netObject.submittedfields[2].FromBytes<Quaternion>();
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (!netView.IsOwned) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) 
            {
                netView.netObject.fields[1] = transform.position.ToBytes();
                netView.netObject.fields[2] = transform.rotation.ToBytes();
                netView.Submit(true);
            }
        }
    }
}
