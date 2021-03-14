using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Megatowel.Multiplex;

namespace Megatowel.NetObject
{
    class NetView : MonoBehaviour
    {
        public bool SceneObject;
        public bool Exclusive;
        public string SceneObjectId;

        public NetObject obj;
        private bool HasInit = false;

        public bool IsOwned
        {
            get
            {
                Debug.Log($"{MultiplexManager.SelfId} == {obj.authority}");
                return MultiplexManager.SelfId == obj.authority;
            }
        }

        private void OnEnable()
        {
            if (SceneObject)
            {
                Guid objId = new Guid(SceneObjectId);
                obj = NetObject.GetNetObject(objId);
                NetManager.allViews[objId] = this;
                HasInit = true;
            }
            
            obj.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.CreateFree);
        }

        // TODO: remove temporary submission method, as this should be automatic.
        // also, we should have neat methods for sending and getting stuff.
        public void Submit(bool getOwner = false)
        {
            obj.SubmitObject(getOwner ? NetFlags.RequestAuthority : NetFlags.CreateFree);
        }

        private void OnDisable()
        {

            HasInit = false;
        }

        internal void NetStart()
        {
            if (!SceneObject)
            {
                if (HasInit)
                {
                    throw new InvalidOperationException("Net View is already active!");
                }
                obj = NetObject.GetNetObject(Guid.NewGuid());
                obj.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.RequestAuthority);
                HasInit = true;
            }
            else
            {
                throw new InvalidOperationException("Scene Objects can't be instantiated.");
            }   
        }

        private void Reset()
        {
            SceneObjectId = Guid.NewGuid().ToString();
        }
    }
}
