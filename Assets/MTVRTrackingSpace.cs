using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTVRTrackingSpace : MonoBehaviour
{
    public Transform Head;
    public Transform LeftController;
    public Transform RightController;
    public Transform XRRigRoot;
    //others eventually if needed

    [Range(0f,1f)]
    public float HeightOffset;


    //delters
    Vector3 previousTransformPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(-Head.localPosition.x, HeightOffset, -Head.localPosition.z);
        XRRigRoot.localPosition += new Vector3(-(transform.localPosition.x - previousTransformPosition.x), 0, -(transform.localPosition.z - previousTransformPosition.z));
        previousTransformPosition = transform.localPosition;
    }

}
