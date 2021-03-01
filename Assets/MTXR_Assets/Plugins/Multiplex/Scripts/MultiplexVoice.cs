using System.Collections;
using System.Collections.Generic;
using POpusCodec;
using POpusCodec.Enums;
using System;
using Megatowel.Debugging;
using UnityEngine;
using System.Text;

namespace Megatowel.Multiplex
{
    public class MultiplexVoice : MonoBehaviour
    {
        public static event Action<MultiplexVoiceData> OnVoiceDecoded;

        private Dictionary<ulong, OpusDecoder> decoders = new Dictionary<ulong, OpusDecoder>();
        private Dictionary<ulong, MultiplexSource> speakers = new Dictionary<ulong, MultiplexSource>();
        private static OpusEncoder encoder;

        void OnEnable()
        {
            encoder = new OpusEncoder(SamplingRate.Sampling48000, Channels.Stereo);
            MultiplexManager.OnEvent += Process;
            MultiplexManager.OnUserConnectEvent += UserDisconnected;
        }

        private void OnDisable()
        {
            MultiplexManager.OnEvent -= Process;
            foreach (var decoder in decoders)
            {
                decoder.Value.Dispose();
            }
            decoders.Clear();
            encoder.Dispose();
        }

        private void Start()
        {
            CreateSink();
        }

        void CreateSink()
        {
            var obj = new GameObject();
            var sink = obj.AddComponent<MultiplexSink>();
        }

        private void UserDisconnected(MultiplexEvent ev)
        {
            if (decoders.ContainsKey(ev.User))
            {
                Destroy(speakers[ev.User]);
                decoders[ev.User].Dispose();
                decoders.Remove(ev.User);
            }
        }

        void Process(MultiplexEvent ev)
        {
            if (ev.Info == "opus")
            {
                if (!decoders.ContainsKey(ev.User))
                {
                    decoders.Add(ev.User, new OpusDecoder(SamplingRate.Sampling48000, Channels.Stereo));
                }

                if (!speakers.ContainsKey(ev.User))
                {
                    var obj = new GameObject();
                    var speaker = obj.AddComponent<MultiplexSource>();
                    speaker.UserId = ev.User;
                    speakers.Add(ev.User, speaker);
                }

                if (ev.Data.text == "END")
                {
                    OnVoiceDecoded?.Invoke(new MultiplexVoiceData(ev.User, new short[1920], decoders[ev.User], true));
                    return;
                }

                OnVoiceDecoded?.Invoke(new MultiplexVoiceData(ev.User, decoders[ev.User].DecodePacket(ev.Data.bytes), decoders[ev.User], false));
            }
        }

        public static void Send(short[] audio, uint channel)
        {
            byte[] data = encoder.Encode(audio);
            MultiplexManager.Send(new MultiplexEvent("opus", new MultiplexData(data), channel));
        }
    }

    public struct MultiplexVoiceData
    {
        public MultiplexVoiceData(ulong user, short[] audio, OpusDecoder state, bool ended)
        {
            User = user;
            Audio = audio;
            State = state;
            Ended = ended;
        }
        public ulong User;
        public short[] Audio;
        public OpusDecoder State;
        public bool Ended;
    }
}
