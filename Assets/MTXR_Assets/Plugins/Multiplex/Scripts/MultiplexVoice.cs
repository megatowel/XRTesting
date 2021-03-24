using System.Collections;
using System.Collections.Generic;
using POpusCodec;
using POpusCodec.Enums;
using System;
using Megatowel.Debugging;
using UnityEngine;
using System.Text;
using Zenject;

namespace Megatowel.Multiplex
{
    public class MultiplexVoice : IInitializable, IDisposable
    {
        private byte[] guidBytes = new byte[16];

        public void Initialize()
        {
            CreateSink();
            MultiplexManager.OnEvent += Process;
        }


        public void Dispose()
        {
            MultiplexManager.OnEvent -= Process;
            foreach (var source in MultiplexSource._instances)
            {
                GameObject.Destroy(source.Value);
            }
        }

        // TODO: More settings for sink, and the ability to have multiple.
        private void CreateSink()
        {
            var obj = new GameObject();
            obj.AddComponent<MultiplexSink>();
            obj.name = $"Sink ID {MultiplexSink.streamId}";
        }

        void Process(MultiplexPacket ev)
        {
            if (ev.Info.bytes[0] == 2)
            {
                Array.Copy(ev.Info.bytes, 1, guidBytes, 0, 16);
                Guid guid = new Guid(guidBytes);

                if (MultiplexSource._instances.ContainsKey(guid))
                {
                    MultiplexSource speaker = MultiplexSource._instances[guid];

                    if (ev.Data.text == "END")
                    {
                        speaker.Process(null, true);
                    }
                    else
                    {
                        speaker.Process(ev.Data.bytes, false);
                    }
                }
            }
        }

        public static void Send(byte[] data, uint channel)
        {
            byte[] info = new byte[17];
            info[0] = 2;
            Array.Copy(MultiplexSink.streamId.ToByteArray(), 0, info, 1, 16);

            MultiplexManager.Send(new MultiplexPacket(new MultiplexData(info), new MultiplexData(data), channel));
        }
    }
}
