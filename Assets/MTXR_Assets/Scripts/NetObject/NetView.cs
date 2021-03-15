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

        public bool IsOwned
        {
            get
            {
                return MultiplexManager.SelfId == netObject.Authority;
            }
        }

        public ulong Authority
        {
            get
            {
                return netObject.Authority;
            }
        }

        // TODO: remove temporary submission method, as this should be automatic.
        // also, we should have neat methods for sending and getting stuff.
        public void Submit(bool getOwner = false)
        {
            netObject.SubmitObject(getOwner ? NetFlags.RequestAuthority : NetFlags.CreateFree);
        }

        public void EditField<T>(byte fieldNum, T obj)
        {
            netObject.SubmitField(fieldNum, obj);
        }

        public T GetField<T>(byte fieldNum)
        {
            return netObject.GetField<T>(fieldNum);
        }

        private void Awake()
        {
            if (SceneObject)
            {
                Guid objId = new Guid(SceneObjectId);
                netObject = NetObject.GetNetObject(objId);
                NetManager.allViews[objId] = this;
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.CreateFree);
            }
            else
            {
                netObject = NetObject.GetNetObject(Guid.NewGuid());
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.RequestAuthority);
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
