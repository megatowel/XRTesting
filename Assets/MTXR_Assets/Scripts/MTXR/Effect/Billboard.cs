using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void OnWillRenderObject()
    {
        if (Camera.current) 
        {
            transform.rotation = Quaternion.LookRotation((transform.position - Camera.current.transform.position).normalized, Vector3.up);
        }
    }
}
