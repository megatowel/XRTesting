using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Zenject;
using Megatowel.Multiplex;
using UnityEngine;
using Megatowel.Multiplex.Extensions;

namespace Megatowel.NetObject
{
    internal class NetManager : IInitializable, IDisposable
    {
        internal static Dictionary<Guid, NetView> allViews = new Dictionary<Guid, NetView>();
        internal static List<NetObject> toSubmit = new List<NetObject>();

        private readonly MemoryStream datareadmem = new MemoryStream();
        private readonly MemoryStream inforeadmem = new MemoryStream();
        private readonly MemoryStream datawritemem = new MemoryStream();
        private readonly MemoryStream infowritemem = new MemoryStream();
        private BinaryReader dataread;
        private BinaryReader inforead;
        private BinaryWriter datawrite;
        private BinaryWriter infowrite;

        public void Initialize()
        {
            dataread = new BinaryReader(datareadmem);
            inforead = new BinaryReader(inforeadmem);
            datawrite = new BinaryWriter(datawritemem);
            infowrite = new BinaryWriter(infowritemem);
            MultiplexManager.OnSetup += () =>
            {
                MultiplexManager.NetworkTick += NetworkTick;
                MultiplexManager.OnEvent += OnNetEvent;
            };
        }

        private void OnNetEvent(MultiplexPacket packet)
        {
            datareadmem.SetLength(0);
            datareadmem.Write(packet.Data.bytes, 0, packet.Data.bytes.Length);
            inforeadmem.SetLength(0);
            inforeadmem.Write(packet.Info.bytes, 0, packet.Info.bytes.Length);
            datareadmem.Seek(0, SeekOrigin.Begin);
            inforeadmem.Seek(0, SeekOrigin.Begin);
            if (packet.Info.bytes.Length > 0 && inforead.ReadByte() == 1)
            {
                dataread.ReadByte();
                foreach (NetObject netobj in NetObject.ReadFromBinaryReaders(dataread, inforead))
                {
                    // it changes objects anyway
                    //Debug.Log($"got {netobj.id} from {netobj.authority}");
                    if (netobj.fields.ContainsKey(1))
                    {
                        Debug.Log(netobj.fields[1].FromBytes<Vector3>());
                    }
                }
            }
        }

        private void NetworkTick()
        {
            datawritemem.SetLength(0);
            infowritemem.SetLength(0);
            datawrite.Write((byte)1);
            infowrite.Write((byte)1);

            foreach (NetObject obj in toSubmit)
            {
                obj.SubmitToBinaryWriters(datawrite, infowrite, obj.flags);
                if (obj.flags.HasFlag(NetFlags.RequestAuthority))
                {
                    obj.flags &= ~NetFlags.RequestAuthority;
                }
            }
            if (toSubmit.Count() != 0)
            {
                MultiplexManager.Send(new MultiplexPacket(new MultiplexData(infowritemem.ToArray()),
                new MultiplexData(datawritemem.ToArray()), 1, MultiplexSendFlags.MT_SEND_RELIABLE));
                toSubmit.Clear();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    static class NetExtensions {
        public static void SubmitObject(this NetObject obj, NetFlags flags)
        {
            if (!flags.Equals(NetFlags.CreateFree))
            {
                obj.flags = flags;
            }
            if (!NetManager.toSubmit.Contains(obj))
            {
                NetManager.toSubmit.Add(obj);
            }
        }
    }
}
