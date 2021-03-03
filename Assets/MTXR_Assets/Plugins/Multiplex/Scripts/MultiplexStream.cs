using Megatowel.Debugging;
using Megatowel.Multiplex.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Megatowel.Multiplex
{
    public class MultiplexStream
    {

        internal ConcurrentQueue<byte[]> sending = new ConcurrentQueue<byte[]>();
        internal ConcurrentQueue<byte> receiving = new ConcurrentQueue<byte>();

        public bool HasData => (receiving.Count >= 28);

        public void SendNext<T>(T obj)
        {
            sending.Enqueue(obj.ToBytes<T>());
        }

        public T ReceiveNext<T>()
        {
            // dumb workaround for getting the total read size
            BinaryReader tempreader = new BinaryReader(new MemoryStream(new byte[128]));
            tempreader.Read<T>();
            long typelen = tempreader.BaseStream.Position;
            tempreader.Close();

            byte[] data = new byte[128];
            for (int i = 0; i < typelen; i++)
            {
                if (!receiving.TryDequeue(out data[i]))
                {
                    MTDebug.Log("oof");
                    break;
                }
            }
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            T obj = reader.Read<T>();
            reader.Close();
            MTDebug.Log($"count after: {receiving.Count}");
            return obj;
        }
    }
}
