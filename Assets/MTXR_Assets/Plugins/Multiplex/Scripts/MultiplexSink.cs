using FMOD;
using FMODUnity;
using Megatowel.Debugging;
using POpusCodec;
using POpusCodec.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Megatowel.Multiplex
{
    public class MultiplexSink : MonoBehaviour
    {
        private int deviceId;
        private static ConcurrentQueue<short> buffer = new ConcurrentQueue<short>();
        private Sound sound;
        private uint lastPos, length, totallength;

        public static FMOD.System RecordSystem;
        public static Guid streamId = Guid.NewGuid();
        private static OpusEncoder encoder = new OpusEncoder(SamplingRate.Sampling48000, Channels.Mono);

        private void Start()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                PermissionCallbacks callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += (ctx) =>
                {
                    Destroy(this);
                };
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
            }

            // Create a new system for recording :pensive:
            int virtchannels = FMODUnity.Settings.Instance.GetVirtualChannels(FMODUnity.FMODPlatform.MobileHigh);
            RecordSystem = new FMOD.System();
            MTDebug.Log($"Creating new FMOD System to grab the microphone through {FMOD.Factory.System_Create(out RecordSystem)}");
            MTDebug.Log($"Setting the new System's OUTPUTTYPE to OPENSL {RecordSystem.setOutput(FMOD.OUTPUTTYPE.OPENSL)}");
            MTDebug.Log($"Initializing the RecordSystem {RecordSystem.init(virtchannels, FMOD.INITFLAGS.NORMAL, IntPtr.Zero)}");
#else
            RecordSystem = RuntimeManager.CoreSystem;
            encoder.EncoderDelay = Delay.Delay40ms;
#endif
            RESULT result;
            result = RecordSystem.getRecordNumDrivers(out int numdrivers, out _);
            if (result != 0)
            {
                MTDebug.LogError($"No recording devices found. {result}");
                return;
            }
            int rate = 0, channels = 0;

            MTDebug.Log($"MultiplexSink numdrivers: {numdrivers}");

            for (int i = 0; i < numdrivers; i++)
            {
                result = RecordSystem.getRecordDriverInfo(i, out string name, 256, out Guid guid, out rate, out _, out channels, out DRIVER_STATE state);
                if (result != 0)
                {
                    MTDebug.LogError($"Failed to get driver info. {result}");
                    return;
                }
                if ((state & DRIVER_STATE.DEFAULT) != 0)
                {
                    deviceId = i;
                    MTDebug.Log($"{guid} : {name}, channels: {channels} [{state}], rate: {rate}");
                }
            }

            channels = 1;

            CREATESOUNDEXINFO exdata = new CREATESOUNDEXINFO();
            exdata.cbsize = Marshal.SizeOf(exdata);
            exdata.numchannels = channels;
            exdata.format = SOUND_FORMAT.PCM16;
            exdata.defaultfrequency = 48000;
            exdata.length = (uint)(exdata.defaultfrequency * sizeof(short) * 0.2); // 200 ms total buffer size

            result = RecordSystem.createSound("Microphone", MODE.OPENUSER | MODE.LOOP_NORMAL | MODE.CREATESAMPLE, ref exdata, out sound);
            if (result != 0)
            {
                MTDebug.LogError($"Failed to create sound. {result}");
                return;
            }
            result = sound.getLength(out totallength, TIMEUNIT.PCM);
            MTDebug.Log($"MTSink totallength: {totallength}");

            if (result != 0)
            {
                MTDebug.LogError($"Failed to get sound length. {result}");
                return;
            }

            result = RecordSystem.recordStart(deviceId, sound, true);
                
            if (result != 0)
            {
                MTDebug.LogError($"Failed to start recording. {result}");
                return;
            }
        }

        private void Update()
        {
            RecordSystem.getRecordPosition(deviceId, out uint position);
            length = (uint)((position >= lastPos) ? (position - lastPos) : (position + totallength - lastPos));
            //MTDebug.Log($"Sink Record length: {length}");
            if (length == 0)
            {
                return;
            }

            sound.getFormat(out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits);
            sound.@lock((uint)(lastPos * sizeof(short) * channels), (uint)(length * sizeof(short) * channels), out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);
            short[] lockbuffer1 = new short[len1 / sizeof(short)];

            Marshal.Copy(ptr1, lockbuffer1, 0, (int)len1 / sizeof(short));
            for (int i = 0; i < (int)len1 / sizeof(short); i++)
            {
                buffer.Enqueue(lockbuffer1[i]);
            }
            if (ptr2.ToInt64() != 0)
            {
                short[] lockbuffer2 = new short[len2 / sizeof(short)];
                Marshal.Copy(ptr2, lockbuffer2, 0, (int)len2 / sizeof(short));
                for (int i = 0; i < (int)len2 / sizeof(short); i++)
                {
                    buffer.Enqueue(lockbuffer2[i]);
                }
            }
            sound.unlock(ptr1, ptr2, len1, len2);
            lastPos = position;

            while (buffer.Count >= encoder.FrameSizePerChannel * channels) // Whenever we get enough to make an opus packet
            {
                short[] opusBuffer = new short[(int)(encoder.FrameSizePerChannel * channels)];
                if (channels == 1 && encoder.InputChannels == Channels.Stereo)
                {
                    for (int i = 0; i < encoder.FrameSizePerChannel; i++)
                    {
                        buffer.TryDequeue(out short popped);
                        opusBuffer[i * 2] = popped;
                        opusBuffer[(i * 2) + 1] = popped;
                    }
                }
                else if (channels == 2 || encoder.InputChannels == Channels.Mono)
                {
                    for (int i = 0; i < encoder.FrameSizePerChannel * channels; i++)
                    {
                        buffer.TryDequeue(out short popped);
                        opusBuffer[i] = popped;
                    }
                }
                MultiplexVoice.Send(encoder.Encode(opusBuffer), 1);
            }
        }

        private void OnDestroy()
        {
            encoder.Dispose();
            sound.release();
        }
    }
}
