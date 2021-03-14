using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Megatowel.NetObject
{
    [Flags]
    enum NetFlags : byte {
        CreateFree = 0, // Mark object to be made and shared freely
        // Free objects will always persist until the instance is gone or object is deleted.
        CreateExclusive = 1 << 0, // Mark object to be made with only the client as the authority (owner).
        // Exclusive objects will be deleted when the owner disconnects.
        RequestAuthority = 1 << 1, // Update object's authority
        NoStore = 1 << 2
    }
    class NetObject
    {
        public Guid id;
        public bool tracking = true;
        private NetFlags _flags;
        public NetFlags flags {
            get {
                if (!tracking && !_flags.HasFlag(NetFlags.NoStore)) {
                    _flags |= NetFlags.NoStore;
                }
                return _flags;
            }
            set {
                _flags = value;
            }
        }
        public ulong authority {
            get {
                if (fields.ContainsKey(0))
                    return BitConverter.ToUInt64(fields[0], 0);
                else
                    return 0;
            }
        }
        public Dictionary<byte, byte[]> fields = new Dictionary<byte, byte[]>(); 
        private Dictionary<byte, byte[]> submittedfields = new Dictionary<byte, byte[]>(); 
        private static Dictionary<Guid, NetObject> instances = new Dictionary<Guid, NetObject>(); 
        private MemoryStream fieldstream = new MemoryStream();
        private BinaryWriter fieldbytes;
        // Generic Constructor for just making a new object.
        public NetObject () {
            id = Guid.NewGuid();
            instances[id] = this;
            fieldbytes = new BinaryWriter(fieldstream);
        }

        private NetObject (Guid objectId) {
            id = objectId;
            fieldbytes = new BinaryWriter(fieldstream);
        }

        public static NetObject GetNetObject(Guid objectId) {
            if (instances.ContainsKey(objectId)) {
                return instances[objectId];
            }
            else {
                NetObject newinstance = new NetObject(objectId);
                instances[objectId] = newinstance;
                return newinstance;
            }
        }

        public void SubmitToBinaryWriters(BinaryWriter data, BinaryWriter info, NetFlags flags, bool submitAll = false) {
            fieldstream.SetLength(0);
            foreach (KeyValuePair<byte, byte[]> pair in fields) {
                if (!submittedfields.ContainsKey(pair.Key) || !submittedfields[pair.Key].Equals(pair.Value)) {
                    fieldbytes.Write(pair.Key);
                    fieldbytes.Write((ushort)pair.Value.Length);
                    fieldbytes.Write(pair.Value);
                    submittedfields[pair.Key] = pair.Value;
                }
            }
            info.Write(id.ToByteArray());
            flags = NetFlags.RequestAuthority;
            info.Write((byte)flags);
            info.Write((ushort)fieldstream.Position);
            data.Write(fieldstream.ToArray());
        }

        public static IEnumerable<NetObject> ReadFromBinaryReaders(BinaryReader data, BinaryReader info) {
            if (((info.BaseStream.Length - info.BaseStream.Position) % 19) != 0) {
                throw new DataMisalignedException();
            }
            long lastPosition;
            ushort dataLength;
            while (info.BaseStream.Length != info.BaseStream.Position) {
                Guid remoteId = new Guid(info.ReadBytes(16));
                NetObject remoteObj = NetObject.GetNetObject(remoteId);
                info.ReadByte(); // ignore it for now ;/
                //remoteObj.flags = (NetFlags)info.ReadByte();

                lastPosition = data.BaseStream.Position;
                dataLength = info.ReadUInt16();
                while ((data.BaseStream.Position - lastPosition) < dataLength) {
                    byte key = data.ReadByte();
                    remoteObj.fields[key] = data.ReadBytes(data.ReadUInt16());
                    remoteObj.submittedfields[key] = remoteObj.fields[key];
                }
                yield return remoteObj;
            }
        }
    }
}