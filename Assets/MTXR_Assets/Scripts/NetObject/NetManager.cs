using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Zenject;
using Megatowel.Multiplex;
using Megatowel.Multiplex.Extensions;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Megatowel.NetObject
{
    public class NetManager : IInitializable, ITickable
    {
        internal static Dictionary<Guid, NetView> allViews = new Dictionary<Guid, NetView>();
        internal static List<NetObject> toSubmit = new List<NetObject>();

        private readonly MemoryStream _dataReadMem = new MemoryStream();
        private readonly MemoryStream _infoReadMem = new MemoryStream();
        private readonly MemoryStream _dataWriteMem = new MemoryStream();
        private readonly MemoryStream _infoWriteMem = new MemoryStream();

        private BinaryReader _dataRead;
        private BinaryReader _infoRead;
        private BinaryWriter _dataWrite;
        private BinaryWriter _infoWrite;

        public const int SendRate = 30;
        private float _sendRateDelta;
        public static event Action NetworkTick;

        public void Initialize()
        {
            _dataRead = new BinaryReader(_dataReadMem);
            _infoRead = new BinaryReader(_infoReadMem);
            _dataWrite = new BinaryWriter(_dataWriteMem);
            _infoWrite = new BinaryWriter(_infoWriteMem);
            MultiplexManager.OnSetup += () =>
            {
                NetworkTick += NetTick;
                MultiplexManager.OnEvent += OnNetEvent;
            };
        }

        public static void SpawnNetAddressable(string address)
        {
            SpawnNetAddressable(address, Guid.NewGuid());
        }

        public static void SpawnNetAddressable(string address, Guid objId)
        {
            allViews[objId] = null;
            Addressables.InstantiateAsync(address).Completed += (obj) =>
            {
                GameObject gameobj = obj.Result;
                NetView view = gameobj.GetComponent<NetView>();
                view.SetNetAddressableObject(objId, address);
            };
        }

        private void OnNetEvent(MultiplexPacket packet)
        {
            // Read data
            _dataReadMem.SetLength(0);
            _dataReadMem.Write(packet.Data.bytes, 0, packet.Data.bytes.Length);

            // Read info
            _infoReadMem.SetLength(0);
            _infoReadMem.Write(packet.Info.bytes, 0, packet.Info.bytes.Length);

            // Seek back to beginning
            _dataReadMem.Seek(0, SeekOrigin.Begin);
            _infoReadMem.Seek(0, SeekOrigin.Begin);

            if (packet.Info.bytes.Length > 0 && _infoRead.ReadByte() == 1)
            {
                _dataRead.ReadByte();
                foreach (NetObject netobj in NetObject.ReadFromBinaryReaders(_dataRead, _infoRead))
                {
                    if (!allViews.ContainsKey(netobj.id) && netobj.fields.ContainsKey(255))
                    {
                        SpawnNetAddressable(netobj.GetField<string>(255), netobj.id);
                    }
                    if (allViews.ContainsKey(netobj.id))
                    {
                        allViews[netobj.id]?.onObjectModify?.Invoke();
                    }
                }
            }
        }

        public void Tick()
        {
            _sendRateDelta += Time.unscaledDeltaTime;
            if (_sendRateDelta > (float)SendRate / 1000)
            {
                _sendRateDelta = 0f;
                NetworkTick?.Invoke();
            }
        }

        private void NetTick()
        {
            _dataWriteMem.SetLength(0);
            _infoWriteMem.SetLength(0);
            _dataWrite.Write((byte)1);
            _infoWrite.Write((byte)1);

            foreach (NetObject obj in toSubmit)
            {
                obj.SubmitToBinaryWriters(_dataWrite, _infoWrite, obj.flags);
                obj.flags &= ~NetFlags.RequestAuthority;
            }
            if (toSubmit.Count() != 0)
            {
                MultiplexManager.Send(new MultiplexPacket(new MultiplexData(_infoWriteMem.ToArray()),
                new MultiplexData(_dataWriteMem.ToArray()), 1, MultiplexSendFlags.Reliable));
                toSubmit.Clear();
            }
        }
    }

    internal static class NetExtensions {
        internal static void SubmitObject(this NetObject netObj, NetFlags flags)
        {
            if (!flags.Equals(NetFlags.CreateFree))
            {
                netObj.flags = flags;
            }
            if (!NetManager.toSubmit.Contains(netObj))
            {
                NetManager.toSubmit.Add(netObj);
            }
        }
        internal static void SubmitField<T>(this NetObject netObj, byte fieldNum, T field)
        {
            netObj.unsubmittedfields[fieldNum] = field.ToBytes<T>();
        }
        internal static T GetField<T>(this NetObject netObj, byte fieldNum)
        {
            return netObj.fields[fieldNum].FromBytes<T>();
        }
    }
}
