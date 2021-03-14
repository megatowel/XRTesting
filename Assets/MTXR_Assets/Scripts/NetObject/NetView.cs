using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Megatowel.Multiplex;

namespace Megatowel.NetObject
{
    public class NetView : MonoBehaviour
    {
        public bool SceneObject;
        public bool Exclusive;
        public string SceneObjectId;

        public NetObject netObject { get; private set; }
        private bool _initialized;

        public bool IsOwned
        {
            get
            {
                return MultiplexManager.SelfId == netObject.authority;
            }
        }

        private void OnEnable()
        {
            if (SceneObject)
            {
                Guid objId = new Guid(SceneObjectId);
                netObject = NetObject.GetNetObject(objId);
                NetManager.allViews[objId] = this;
                _initialized = true;
            }

            netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.CreateFree);
        }

        // TODO: remove temporary submission method, as this should be automatic.
        // also, we should have neat methods for sending and getting stuff.
        public void Submit(bool getOwner = false)
        {
            netObject.SubmitObject(getOwner ? NetFlags.RequestAuthority : NetFlags.CreateFree);
        }

        private void OnDisable()
        {

            _initialized = false;
        }

        internal void NetStart()
        {
            if (!SceneObject)
            {
                if (_initialized)
                {
                    throw new InvalidOperationException("Net View is already active!");
                }
                netObject = NetObject.GetNetObject(Guid.NewGuid());
                netObject.SubmitObject(Exclusive ? NetFlags.CreateExclusive : NetFlags.RequestAuthority);
                _initialized = true;
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
