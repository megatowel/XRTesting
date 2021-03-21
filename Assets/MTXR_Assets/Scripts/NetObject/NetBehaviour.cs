using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Megatowel.NetObject {
    public abstract class NetBehaviour : MonoBehaviour 
    {
        /// <summary>
        /// Reference to this object's NetView.<br/>
        /// Will first look for NetViews on itself and parents, and cache what it finds.<br/>
        /// If no NetView was found, an assertion will fail.
        /// </summary>
        protected NetView netView
        { 
            get 
            {
                if (!_netView) {
                    _netView = gameObject.GetComponent<NetView>();
                    if (!_netView) {
                        _netView = gameObject.GetComponentInParent<NetView>();
                    }
                }
                Assert.IsNotNull<NetView>(_netView, $"NetBehaviour on object {gameObject.name} could not locate a NetView. Please put a NetView component on it or a parent object.");
                return _netView;
            }
        }
        private NetView _netView;
    }
}