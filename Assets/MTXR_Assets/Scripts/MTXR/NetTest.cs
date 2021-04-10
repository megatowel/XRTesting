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

    private Vector3 _lastPositionOnNet = Vector3.zero;
    private Quaternion _lastRotationOnNet = Quaternion.identity;

    private Vector3 _lastNetPosition;
    private Quaternion _lastNetRotation;

    private float _lerpNumber = 0.0f;
    private const float _lerpMax = 5.0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        netView.onObjectModify += () => {
            _lastPositionOnNet = transform.position;
            _lastRotationOnNet = transform.rotation;

            // This event runs before the fields get changed.
            _lastNetPosition = netView.GetLocalField<Vector3>(1);
            _lastNetRotation = netView.GetLocalField<Quaternion>(2);

            _lerpNumber = 0.0f;
        };
    }
    // Update is called once per frame, as you might know by now :3
    void Update()
    {
        _lerpNumber += Time.deltaTime * NetManager.SendRate;
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
            _rb.velocity = Vector3.Lerp(_rb.velocity, netView.GetField<Vector3>(3), Time.deltaTime * 20);
            _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, netView.GetField<Vector3>(4), Time.deltaTime * 20);
            if (_rb.velocity.magnitude + _rb.angularVelocity.magnitude < 0.015)
            {
                _lerpNumber = 1.0f;
            }
            // This is for using two different lerp A points. This is so it doesn't build up incorrect extrapolation and "swing away".
            if (_lerpNumber <= 1.0f)
            {
                transform.position = Vector3.Lerp(_lastPositionOnNet, netView.GetField<Vector3>(1), _lerpNumber);
                transform.rotation = Quaternion.Lerp(_lastRotationOnNet, netView.GetField<Quaternion>(2), _lerpNumber);
            }
            // We only have to null check at least the last net position.
            else if (_lastNetPosition != null)
            {
                transform.position = Vector3.LerpUnclamped(_lastNetPosition, netView.GetField<Vector3>(1), _lerpNumber < _lerpMax ? _lerpNumber : 1.0f);
                transform.rotation = Quaternion.LerpUnclamped(_lastNetRotation, netView.GetField<Quaternion>(2), _lerpNumber < _lerpMax ? _lerpNumber : 1.0f);
            }
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
