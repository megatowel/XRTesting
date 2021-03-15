using System;
using System.IO;
using System.Collections.Generic;

namespace Megatowel.NetObject
{
    [Flags]
    internal enum NetFlags : byte
    {
        CreateFree = 0, // Mark object to be made and shared freely
        // Free objects will always persist until the instance is gone or object is deleted.
        CreateExclusive = 1 << 0, // Mark object to be made with only the client as the authority (owner).
        // Exclusive objects will be deleted when the owner disconnects.
        RequestAuthority = 1 << 1, // Update object's authority
        NoStore = 1 << 2
    }

    public class NetObject : IDisposable
    {
        public Guid id;
        public bool synced = false;

        internal NetFlags flags;

        public ulong Authority
        {
            get
            {
                if (fields.ContainsKey(0))
                    return BitConverter.ToUInt64(fields[0], 0);
                else
                    return 0;
            }
        }

        public Dictionary<byte, byte[]> fields = new Dictionary<byte, byte[]>();

        internal Dictionary<byte, byte[]> unsubmittedfields = new Dictionary<byte, byte[]>();

        private static Dictionary<Guid, NetObject> _instances = new Dictionary<Guid, NetObject>();
        private MemoryStream _fieldStream = new MemoryStream();
        private BinaryWriter _fieldBytes;

        // Generic Constructor for just making a new object.
        public NetObject()
        {
            id = Guid.NewGuid();
            _instances[id] = this;
            _fieldBytes = new BinaryWriter(_fieldStream);
        }

        private NetObject(Guid objectId)
        {
            id = objectId;
            _fieldBytes = new BinaryWriter(_fieldStream);
        }

        public static NetObject GetNetObject(Guid objectId)
        {
            if (_instances.ContainsKey(objectId))
            {
                return _instances[objectId];
            }
            else
            {
                NetObject newinstance = new NetObject(objectId);
                _instances[objectId] = newinstance;
                return newinstance;
            }
        }

        internal void SubmitToBinaryWriters(BinaryWriter data, BinaryWriter info, NetFlags flags, bool submitAll = false)
        {
            _fieldStream.SetLength(0);
            foreach (KeyValuePair<byte, byte[]> pair in unsubmittedfields)
            {
                if (submitAll || !fields.ContainsKey(pair.Key) || !fields[pair.Key].Equals(pair.Value))
                {
                    _fieldBytes.Write(pair.Key);
                    _fieldBytes.Write((ushort)pair.Value.Length);
                    _fieldBytes.Write(pair.Value);
                    // redoing this submitted fields thing. it may just end up being "server side" fields
                    // submittedfields[pair.Key] = pair.Value;
                }
            }
            info.Write(id.ToByteArray());
            info.Write((byte)flags);
            info.Write((ushort)_fieldStream.Position);
            data.Write(_fieldStream.ToArray());
            unsubmittedfields.Clear();
        }

        internal static IEnumerable<NetObject> ReadFromBinaryReaders(BinaryReader data, BinaryReader info)
        {
            if (((info.BaseStream.Length - info.BaseStream.Position) % 19) != 0)
            {
                throw new DataMisalignedException();
            }
            long lastPosition;
            ushort dataLength;
            while (info.BaseStream.Length != info.BaseStream.Position)
            {
                Guid remoteId = new Guid(info.ReadBytes(16));
                NetObject remoteObj = NetObject.GetNetObject(remoteId);
                info.ReadByte(); // ignore it for now ;/
                //remoteObj.flags = (NetFlags)info.ReadByte();

                lastPosition = data.BaseStream.Position;
                dataLength = info.ReadUInt16();
                while ((data.BaseStream.Position - lastPosition) < dataLength)
                {
                    byte key = data.ReadByte();
                    remoteObj.fields[key] = data.ReadBytes(data.ReadUInt16());
                }
                yield return remoteObj;
            }
        }

        public void Dispose() {
            _fieldStream.Dispose();
            _fieldBytes.Dispose();
        }
    }
}