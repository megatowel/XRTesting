using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Megatowel.NetObject;
using Megatowel.Multiplex.Extensions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetView))]
public class NetTest : MonoBehaviour
{
    NetView netView;
    // Start is called before the first frame update
    void Start()
    {
        netView = GetComponent<NetView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.isPressed)
        {
            netView.Submit(true);
        }
        if (netView.IsOwned)
        {
            Debug.Log("we sendin");
            netView.obj.fields[1] = transform.position.ToBytes();
            netView.obj.fields[2] = transform.rotation.ToBytes();
            netView.Submit();
        }
        else
        {
            transform.position = netView.obj.fields[1].FromBytes<Vector3>();
            transform.rotation = netView.obj.fields[2].FromBytes<Quaternion>();
        }
    }
}
