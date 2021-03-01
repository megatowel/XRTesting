using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMODUnity;
using POpusCodec;
using UnityEngine;
using Megatowel.Debugging;
using AOT;

namespace Megatowel.Multiplex
{
    public class MultiplexSource : MonoBehaviour
    {
        public ulong UserId;

        private Sound sound;
        private ChannelGroup master;

        private static Dictionary<ulong, OpusDecoder> states = new Dictionary<ulong, OpusDecoder>();
        private static Dictionary<ulong, ConcurrentQueue<short>> buffers = new Dictionary<ulong, ConcurrentQueue<short>>();
        private static Dictionary<ulong, bool> validStates = new Dictionary<ulong, bool>();

        private void OnEnable()
        {
            MultiplexVoice.OnVoiceDecoded += Process;
        }

        private void OnDisable()
        {
            sound.release();
            MultiplexVoice.OnVoiceDecoded -= Process;
        }

        private void Process(MultiplexVoiceData data)
        {
            if (UserId != data.User)
                return;
            // This is all because of some stupid thread crap
            if (!states.ContainsKey(UserId))
                states.Add(UserId, data.State);

            if (!buffers.ContainsKey(UserId))
                buffers.Add(UserId, new ConcurrentQueue<short>());

            if (!validStates.ContainsKey(UserId))
                validStates.Add(UserId, false);

            validStates[UserId] = !data.Ended;

            if (data.Audio != null)
            {
                sound.getOpenState(out OPENSTATE openstate, out _, out _, out _);

                foreach (short pcm in data.Audio)
                {
                    buffers[UserId].Enqueue(pcm);
                }

            }

            if (sound.handle.ToInt32() == 0)
            {
                UserData userData = new UserData(data.User);
                IntPtr userptr = Marshal.AllocHGlobal(Marshal.SizeOf(userData));
                Marshal.StructureToPtr(userData, userptr, true);
                CREATESOUNDEXINFO info = new CREATESOUNDEXINFO
                {
                    cbsize = Marshal.SizeOf(new CREATESOUNDEXINFO()),
                    format = SOUND_FORMAT.PCM16,
                    length = 960 * 2,
                    numchannels = 2,
                    defaultfrequency = 48000,
                    decodebuffersize = 960 * 2,
                    pcmreadcallback = pcmcallback,
                    userdata = userptr

                };

                RuntimeManager.CoreSystem.createSound(data.User.ToString(), MODE.OPENUSER | MODE.LOOP_NORMAL | MODE._3D | MODE.CREATESTREAM, ref info, out sound);
                RuntimeManager.CoreSystem.getMasterChannelGroup(out master);
                RuntimeManager.CoreSystem.playSound(sound, master, false, out _);
            }
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
            ulong userid = userData.User;
            short[] temp = new short[datalen / 2];
            if (buffers[userid].Count < datalen / 2 && !validStates[userid])
            {
                // It actually may give us old data back, so we need to start fresh.
                Marshal.Copy(temp, 0, data, (int)datalen / 2);

                return RESULT.OK;
            }

            while (buffers[userid].Count < datalen / 2)
            {
                if (validStates[userid])
                {
                    foreach (short pcm in states[userid].DecodePacketLost())
                    {
                        buffers[userid].Enqueue(pcm);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < (int)datalen / 2; i++)
            {
                buffers[userid].TryDequeue(out temp[i]);
            }
            Marshal.Copy(temp, 0, data, (int)datalen / 2);
            return RESULT.OK; // This doesn't actually matter
        }

        public struct UserData
        {
            public UserData(ulong user)
            {
                this.User = user;
            }
            public ulong User;
        }
    }
}
