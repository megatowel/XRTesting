using System;
using System.IO;
using UnityEngine;

namespace Megatowel.Multiplex.Extensions
{
    public static class MultiplexExtensions
    {
        // array/list serialization i guess
        public static byte[] ToBytes<T>(this T obj)
        {
            if (obj == null)
            {
                return null;
            }

            MemoryStream memStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memStream);
            
            if (typeof(T) == typeof(bool)) {
                writer.Write((bool)(object)obj);
            }
            if (typeof(T) == typeof(byte)) {
                writer.Write((byte)(object)obj);
            }
            if (typeof(T) == typeof(sbyte)) {
                writer.Write((sbyte)(object)obj);
            }
            if (typeof(T) == typeof(decimal)) {
                writer.Write((decimal)(object)obj);
            }
            if (typeof(T) == typeof(double)) {
                writer.Write((double)(object)obj);
            }
            if (typeof(T) == typeof(short)) {
                writer.Write((short)(object)obj);
            }
            if (typeof(T) == typeof(int)) {
                writer.Write((int)(object)obj);
            }
            if (typeof(T) == typeof(long)) {
                writer.Write((long)(object)obj);
            }
            if (typeof(T) == typeof(ushort)) {
                writer.Write((ushort)(object)obj);
            }
            if (typeof(T) == typeof(uint)) {
                writer.Write((uint)(object)obj);
            }
            if (typeof(T) == typeof(ulong)) {
                writer.Write((ulong)(object)obj);
            }
            if (typeof(T) == typeof(float)) {
                writer.Write((float)(object)obj);
            }
            if (typeof(T) == typeof(string)) {
                writer.Write((string)(object)obj);
            }
            if (typeof(T) == typeof(byte[])) {
                byte[] bytes = (byte[])(object)obj;
                writer.Write((ushort)(object)bytes.Length);
                writer.Write(bytes);
            }
            if (typeof(T) == typeof(Vector2)) {
                Vector2 vector = (Vector2)(object)obj;
                writer.Write(vector.x);
                writer.Write(vector.y);
            }
            if (typeof(T) == typeof(Vector3)) {
                Vector3 vector = (Vector3)(object)obj;
                writer.Write(vector.x);
                writer.Write(vector.y);
                writer.Write(vector.z);
            }
            if (typeof(T) == typeof(Vector4)) {
                Vector4 vector = (Vector4)(object)obj;
                writer.Write(vector.x);
                writer.Write(vector.y);
                writer.Write(vector.z);
                writer.Write(vector.w);
            }
            if (typeof(T) == typeof(Quaternion)) {
                Quaternion vector = (Quaternion)(object)obj;
                writer.Write(vector.x);
                writer.Write(vector.y);
                writer.Write(vector.z);
                writer.Write(vector.w);
            }

            byte[] sourceArray = memStream.GetBuffer();
            byte[] truncArray = new byte[memStream.Position];

            Array.Copy(sourceArray, truncArray, truncArray.Length);
            return truncArray;
        }

        public static T FromBytes<T>(this byte[] obj)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream());
            reader.BaseStream.Write(obj, 0, obj.Length);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return reader.Read<T>();
        }

        // extend BinaryReader to read epicly
        public static T Read<T>(this BinaryReader reader)
        {
            if (typeof(T) == typeof(bool)) {
                bool obj = reader.ReadBoolean();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(byte)) {
                byte obj = reader.ReadByte();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(sbyte)) {
                sbyte obj = reader.ReadSByte();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(decimal)) {
                decimal obj = reader.ReadDecimal();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(double)) {
                double obj = reader.ReadDouble();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(short)) {
                short obj = reader.ReadInt16();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(int)) {
                int obj = reader.ReadInt32();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(long)) {
                long obj = reader.ReadInt64();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(ushort)) {
                ushort obj = reader.ReadUInt16();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(uint)) {
                uint obj = reader.ReadUInt32();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(ulong)) {
                ulong obj = reader.ReadUInt64();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(float)) {
                float obj = reader.ReadSingle();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(string)) {
                string obj = reader.ReadString();
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(byte[])) {
                ushort length = reader.ReadUInt16();
                byte[] obj = reader.ReadBytes(length);
                return (T)(object)obj;
            }
            if (typeof(T) == typeof(Vector2)) {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                return (T)(object)new Vector2(x, y);
            }
            if (typeof(T) == typeof(Vector3)) {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                return (T)(object)new Vector3(x, y, z);
            }
            if (typeof(T) == typeof(Vector4)) {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                float w = reader.ReadSingle();
                return (T)(object)new Vector4(x, y, z, w);
            }
            if (typeof(T) == typeof(Quaternion)) {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                float w = reader.ReadSingle();
                return (T)(object)new Quaternion(x, y, z, w);
            }
            return default(T);
        }
    }
}
