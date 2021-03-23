using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megatowel.NetObject;
using MTXR.Player;
using UnityEngine.InputSystem;
using System.Diagnostics;

[RequireComponent(typeof(Rigidbody))]
public class NetTest : NetBehaviour
{
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame, as you might know by now :3
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            netView.RequestAuthority();
            UnityEngine.Debug.Log("we ownin");
        }
        if (netView.authorityStatus != AuthorityStatus.RemoteAuthority)
        {
            _rb.constraints = RigidbodyConstraints.None;
            netView.EditField(1, transform.position);
            netView.EditField(2, transform.rotation);
            netView.EditField(3, _rb.velocity);
            netView.EditField(4, _rb.angularVelocity);
            netView.Submit();
        }
        else if (netView.Authority != 0)
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = Vector3.Lerp(transform.position, netView.GetField<Vector3>(1), Time.deltaTime * 20);
            transform.rotation = Quaternion.Lerp(transform.rotation, netView.GetField<Quaternion>(2), Time.deltaTime * 20);
            _rb.velocity = Vector3.Lerp(_rb.velocity, netView.GetField<Vector3>(3), Time.deltaTime * 20);
            _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, netView.GetField<Vector3>(4), Time.deltaTime * 20);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!netView.IsOwned)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                netView.EditField(1, transform.position);
                netView.EditField(2, transform.rotation);
                netView.EditField(3, _rb.velocity);
                netView.RequestAuthority();
            }
        }
    }
}
