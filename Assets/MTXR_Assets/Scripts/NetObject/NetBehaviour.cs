using System;
using UnityEngine;

namespace Megatowel.NetObject {

    [RequireComponent(typeof(NetView))]
    public abstract class NetBehaviour : MonoBehaviour 
    {
        protected NetView netView
        { 
            get 
            {
                if (!_netView) {
                    _netView = gameObject.GetComponent<NetView>();
                }
                return _netView;
            }
        }
        private NetView _netView;
    }
}