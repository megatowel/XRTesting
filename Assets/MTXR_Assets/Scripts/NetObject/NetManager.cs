using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Zenject;
using Megatowel.Multiplex;
using Megatowel.Multiplex.Extensions;

namespace Megatowel.NetObject
{
    internal class NetManager : IInitializable
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

        public void Initialize()
        {
            _dataRead = new BinaryReader(_dataReadMem);
            _infoRead = new BinaryReader(_infoReadMem);
            _dataWrite = new BinaryWriter(_dataWriteMem);
            _infoWrite = new BinaryWriter(_infoWriteMem);
            MultiplexManager.OnSetup += () =>
            {
                MultiplexManager.NetworkTick += NetworkTick;
                MultiplexManager.OnEvent += OnNetEvent;
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
                    allViews[netobj.id]?.onObjectModify?.Invoke(); 
                }
            }
        }

        private void NetworkTick()
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
