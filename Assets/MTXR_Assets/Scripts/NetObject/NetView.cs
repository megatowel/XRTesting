using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Megatowel.Multiplex;

namespace Megatowel.NetObject
{
    [DisallowMultipleComponent]
    public class NetView : MonoBehaviour
    {
        public bool SceneObject;
        public bool Exclusive;
        public string SceneObjectId;

        public NetObject netObject { get; private set; }
        public Action onObjectModify;

        public AuthorityStatus authorityStatus = AuthorityStatus.RemoteAuthority;
        private float _requestHangTime = 0.0f;
        private const float _maxHangTime = 1.0f;

        public bool IsOwned
        {
            get
            {
                if (netObject == null || MultiplexManager.Self == 0)
                {
                    return false;
                }
                return MultiplexManager.Self == netObject.Authority;
            }
        }

        public ulong Authority
        {
            get
            {
                return netObject.Authority;
            }
        }

        public void RequestAuthority()
        {
            _requestHangTime = 0.0f;
            authorityStatus = AuthorityStatus.RequestingAuthority;
            netObject.SubmitObject(NetFlags.RequestAuthority);
        }

        private void Update()
        {
            if (IsOwned)
            {
                authorityStatus = AuthorityStatus.LocalAuthority;
            }
            if (authorityStatus == AuthorityStatus.RequestingAuthority) {
                _requestHangTime += Time.deltaTime;
                if (_requestHangTime >= _maxHangTime)
                {
                    authorityStatus = AuthorityStatus.RemoteAuthority;
                }
            }
            else if (!IsOwned)
            {
                authorityStatus = AuthorityStatus.RemoteAuthority;
            }
        }

        public void Submit()
        {
            netObject.SubmitObject(NetFlags.CreateFree);
        }

        public void EditField<T>(byte fieldNum, T obj)
        {
            netObject.SubmitField(fieldNum, obj);
        }

        public T GetField<T>(byte fieldNum)
        {
            return netObject.GetField<T>(fieldNum);
        }

        public T GetLocalField<T>(byte fieldNum)
        {
            if (netObject.localFields.ContainsKey(fieldNum))
                return netObject.GetField<T>(fieldNum, true);
            else
                return default(T);
        }

        private void Start()
        {
            if (SceneObject)
            {
                Guid objId = new Guid(SceneObjectId);
                netObject = NetObject.GetNetObject(objId);
                NetManager.allViews[objId] = this;
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.CreateFree);
            }
            /*else if (netObject != null)
            {
                netObject = NetObject.GetNetObject(Guid.NewGuid());
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.RequestAuthority);
            }*/
        }

        internal void SetNetAddressableObject(Guid objId, string address)
        {
            Debug.Log(address);
            if (netObject == null)
            {
                Debug.Log("yay");
                netObject = NetObject.GetNetObject(objId);
                NetManager.allViews[objId] = this;
                netObject.SubmitField(255, address);
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.RequestAuthority);
                authorityStatus = AuthorityStatus.RequestingAuthority;
            }
        }

        private void OnDestroy()
        {
            NetManager.allViews.Remove(netObject.id);
        }

        private void Reset()
        {
            SceneObjectId = Guid.NewGuid().ToString();
        }
    }
}
