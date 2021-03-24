using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMODUnity;
using POpusCodec;
using POpusCodec.Enums;
using UnityEngine;
using Megatowel.Debugging;
using AOT;

namespace Megatowel.Multiplex
{
    public class MultiplexSource : MonoBehaviour
    {
        public Guid Id;

        private Sound sound;
        private ChannelGroup master;
        private Channel channel;

        internal OpusDecoder Decoder = new OpusDecoder(SamplingRate.Sampling48000, Channels.Mono);
        internal ConcurrentQueue<short> Buffer = new ConcurrentQueue<short>();
        internal bool State = false;

        internal static Dictionary<Guid, MultiplexSource> _instances = new Dictionary<Guid, MultiplexSource>();

        public static MultiplexSource CreateSource(Guid id, Transform parent = null)
        {
            var obj = new GameObject();
            MultiplexSource speaker = obj.AddComponent<MultiplexSource>();
            if (parent)
            {
                obj.transform.parent = parent;
            }
            speaker.Id = id;
            MultiplexSource._instances[id] = speaker;
            obj.name = $"Source ID {id}";
            return speaker;
        }

        internal void Process(byte[] data, bool state)
        {
            State = state;

            if (data != null)
            {
                sound.getOpenState(out OPENSTATE openstate, out _, out _, out _);

                short[] _audio = Decoder.DecodePacket(data);

                foreach (short pcm in _audio)
                {
                    Buffer.Enqueue(pcm);
                }

            }

            if (sound.handle.ToInt32() == 0)
            {
                UserData userData = new UserData(Id);
                IntPtr userptr = Marshal.AllocHGlobal(Marshal.SizeOf(userData));
                Marshal.StructureToPtr(userData, userptr, true);
                CREATESOUNDEXINFO info = new CREATESOUNDEXINFO
                {
                    cbsize = Marshal.SizeOf(new CREATESOUNDEXINFO()),
                    format = SOUND_FORMAT.PCM16,
                    length = 960 * 2,
                    numchannels = 1,
                    defaultfrequency = 48000,
                    decodebuffersize = 960 * 2,
                    pcmreadcallback = pcmcallback,
                    userdata = userptr

                };

                RuntimeManager.CoreSystem.createSound(Id.ToString(), MODE.OPENUSER | MODE.LOOP_NORMAL | MODE._3D | MODE.CREATESTREAM, ref info, out sound);
                RuntimeManager.CoreSystem.getMasterChannelGroup(out master);
                RuntimeManager.CoreSystem.playSound(sound, master, false, out channel);
            }
        }

        void Update()
        {
            var attributes = RuntimeUtils.To3DAttributes(gameObject);
            channel.set3DAttributes(ref attributes.position, ref attributes.velocity);
        }

        [MonoPInvokeCallback(typeof(RESULT))]
        private static RESULT pcmcallback(IntPtr soundptr, IntPtr data, uint datalen)
        {
            Sound newsound = new Sound
            {
                handle = soundptr
            };
            newsound.getUserData(out IntPtr userptr);
            UserData userData = (UserData)Marshal.PtrToStructure(userptr, typeof(UserData));
            MultiplexSource instance = _instances[userData.User];
            short[] temp = new short[datalen / 2];
            if (instance.Buffer.Count < datalen / 2 && !instance.State)
            {
                // It actually may give us old data back, so we need to start fresh.
                Marshal.Copy(temp, 0, data, (int)datalen / 2);

                return RESULT.OK;
            }

            while (instance.Buffer.Count < datalen / 2)
            {
                if (instance.State)
                {
                    foreach (short pcm in instance.Decoder.DecodePacketLost())
                    {
                        instance.Buffer.Enqueue(pcm);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < (int)datalen / 2; i++)
            {
                instance.Buffer.TryDequeue(out temp[i]);
            }
            Marshal.Copy(temp, 0, data, (int)datalen / 2);
            return RESULT.OK; // This doesn't actually matter
        }

        public void OnDestroy()
        {
            _instances.Remove(Id);
            Decoder.Dispose();
            sound.release();
        }

        public struct UserData
        {
            public UserData(Guid user)
            {
                this.User = user;
            }
            public Guid User;
        }
    }
}
