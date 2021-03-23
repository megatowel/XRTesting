using System;
using System.IO;
using System.Linq;
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
        public bool remote = false;

        internal NetFlags flags;

        public ulong Authority
        {
            get
            {
                if (remoteFields.ContainsKey(0))
                    return BitConverter.ToUInt64(remoteFields[0], 0);
                else
                    return 0;
            }
        }

        public Dictionary<byte, byte[]> remoteFields = new Dictionary<byte, byte[]>();

        public Dictionary<byte, byte[]> localFields = new Dictionary<byte, byte[]>();

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
            foreach (KeyValuePair<byte, byte[]> pair in localFields)
            {
                if (pair.Key == 0)
                {
                    continue; // Skip the authority key.
                }
                if (submitAll || !remoteFields.ContainsKey(pair.Key) || !remoteFields[pair.Key].SequenceEqual(pair.Value))
                {
                    _fieldBytes.Write(pair.Key);
                    _fieldBytes.Write((ushort)pair.Value.Length);
                    _fieldBytes.Write(pair.Value);
                }
            }
            // Don't push to writers if we did nothing. But push if we have flags to give, isn't on remote, or if forced.
            if (_fieldStream.Position != 0 || flags != NetFlags.CreateFree || submitAll || !remote)
            {
                info.Write(id.ToByteArray());
                info.Write((byte)flags);
                info.Write((ushort)_fieldStream.Position);
                data.Write(_fieldStream.ToArray());
            }
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
                remoteObj.remote = true;
                info.ReadByte(); // ignore it for now ;/
                //remoteObj.flags = (NetFlags)info.ReadByte();

                lastPosition = data.BaseStream.Position;
                dataLength = info.ReadUInt16();
                while ((data.BaseStream.Position - lastPosition) < dataLength)
                {
                    byte key = data.ReadByte();
                    remoteObj.remoteFields[key] = data.ReadBytes(data.ReadUInt16());
                }
                yield return remoteObj;
            }
        }

        internal void SyncLocalToRemote()
        {
            localFields.Clear();
            foreach (KeyValuePair<byte, byte[]> pair in remoteFields)
            {
                if (pair.Key == 0)
                {
                    continue; // Skip the authority key.
                }
                localFields[pair.Key] = pair.Value;
            }
        }

        public void Dispose() {
            _fieldStream.Dispose();
            _fieldBytes.Dispose();
        }
    }
}