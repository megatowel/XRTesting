using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megatowel.Multiplex;
using Megatowel.NetObject;
using Megatowel.Multiplex.Extensions;
using MTXR.Player;
using UnityEngine.InputSystem;

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
            netView.Submit(true);
            Debug.Log("we ownin");
        }
        if (netView.IsOwned)
        {
            _rb.constraints = RigidbodyConstraints.None;
            netView.netObject.fields[1] = transform.position.ToBytes();
            netView.netObject.fields[2] = transform.rotation.ToBytes();
            netView.netObject.fields[3] = _rb.velocity.ToBytes();
            netView.Submit();
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = Vector3.Lerp(transform.position, netView.netObject.submittedfields[1].FromBytes<Vector3>(), Time.deltaTime * 15);
            transform.rotation = Quaternion.Lerp(transform.rotation, netView.netObject.submittedfields[2].FromBytes<Quaternion>(), Time.deltaTime * 15);
            _rb.velocity = Vector3.Lerp(_rb.velocity, netView.netObject.submittedfields[3].FromBytes<Vector3>(), Time.deltaTime * 15);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!netView.IsOwned)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                netView.netObject.fields[1] = transform.position.ToBytes();
                netView.netObject.fields[2] = transform.rotation.ToBytes();
                netView.netObject.fields[3] = _rb.velocity.ToBytes();
                netView.Submit(true);
            }
        }
    }
}
