using Megatowel.NetObject;
using UnityEngine;

public class NetReserveTest : NetBehaviour
{
    /* example
    private byte _fieldId1;
    private byte _fieldId2;

    public void Start() 
    {
        _fieldId1 = netView.ReserveField<float>();
        _fieldId2 = netView.ReserveField<float>();
        Debug.Log(_fieldId1);
        Debug.Log(_fieldId2);
    }

    public void Update()
    {
        if (netView.IsOwned) 
        {
            netView.EditField<float>(_fieldId1, Time.deltaTime * 2);
            netView.EditField<float>(_fieldId2, Time.deltaTime * 4);
        }
        else 
        {
            Debug.Log(netView.GetField<float>(_fieldId1));
            Debug.Log(netView.GetField<float>(_fieldId2));
        }
    }
    */
}