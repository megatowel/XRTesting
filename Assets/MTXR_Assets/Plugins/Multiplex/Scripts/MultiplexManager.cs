using Megatowel.Debugging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Zenject;
using UnityEngine;

namespace Megatowel.Multiplex
{
    public class MultiplexManager : IInitializable, IDisposable, ITickable
    {
        //sendrate
        public const int sendrate = 30;
        private float sendratedelta;

        public static event Action<MultiplexEvent> OnEvent;
        public static event Action<MultiplexErrors> OnError;
        public static event Action<MultiplexEvent> OnUserConnectEvent;
        public static event Action<MultiplexEvent> OnUserDisconnectEvent;
        public static event Action OnSetup;
        public static event Action OnDisconnect;
        public static event Action NetworkTick;
        public static ulong SelfId;

        private IntPtr multiplex;
        private static ConcurrentQueue<MultiplexEvent> sendQueue = new ConcurrentQueue<MultiplexEvent>();
        private static ConcurrentDictionary<uint, List<ulong>> userLists = new ConcurrentDictionary<uint, List<ulong>>();

        private readonly string hostServer = "104.37.189.85";
        private readonly int hostPort = 3000;
        // SOUP private readonly string hostServer = "108.230.44.131";
        // SOUP private readonly int hostPort = 3000;
        // ;>

        public void Initialize()
        {
            if (Multiplex.init_enet() != 0)
            {
                throw new MultiplexException("Failed to initialize ENet.");
            }
            Setup(hostServer);

            OnSetup += () =>
            {
                Multiplex.c_bind_channel(multiplex, 1, 1);
            };

            OnError += (error) =>
            {
                Setup(hostServer);
            };

            OnDisconnect += () =>
            {
                Setup(hostServer);
            };

            OnEvent += (MultiplexEvent ev) =>
            {
                if (ev.Info == "chat")
                {
                    MTDebug.Log(ev.Data.text);
                }
            };
        }

        public void Setup(string host)
        {
            multiplex = Multiplex.c_make_client();
            if (Multiplex.c_setup(multiplex, Encoding.UTF8.GetBytes(host), hostPort) == 0)
            {
                MTDebug.Log("Starting Multiplex");
            }
        }

        public void Dispose()
        {
        }

        private void Process()
        {
            try
            {
                while (multiplex.ToInt64() != 0)
                {
                    var ev = Multiplex.c_process_event(multiplex, 0);
                    if (ev.Error == 0)
                    {
                        byte[] data = new byte[(int)ev.DataSize];
                        byte[] info = new byte[(int)ev.InfoSize];
                        if (ev.DataSize != 0)
                        {
                            Marshal.Copy(ev.Data, data, 0, (int)ev.DataSize);
                        }
                        if (ev.InfoSize != 0)
                        {
                            Marshal.Copy(ev.Info, info, 0, (int)ev.InfoSize);
                        }

                        switch (ev.EventType)
                        {
                            case Multiplex.MultiplexEventType.UserSetup:
                                SelfId = ev.FromUserId;
                                OnSetup?.Invoke();
                                break;

                            case Multiplex.MultiplexEventType.UserMessage:
                                OnEvent?.Invoke(new MultiplexEvent(ev.FromUserId, Encoding.UTF8.GetString(info), new MultiplexData(data), ev.ChannelId));
                                break;

                            case Multiplex.MultiplexEventType.Disconnected:
                                OnDisconnect?.Invoke();
                                throw new MultiplexException("Lost connection to server.");

                            case Multiplex.MultiplexEventType.InstanceConnected:
                                long[] userHolder = new long[ev.UserIdsSize];
                                Marshal.Copy(ev.UserIds, userHolder, 0, (int)ev.UserIdsSize);
                                ulong[] users = new ulong[ev.UserIdsSize];
                                for (int i = 0; i < users.Length; i++)
                                {
                                    users[i] = (ulong)userHolder[i];
                                }
                                userLists[ev.ChannelId] = new List<ulong>(users);
                                break;

                            case Multiplex.MultiplexEventType.InstanceUserUpdate:
                                if (ev.FromUserId != SelfId)
                                {
                                    if (ev.InstanceId != 0)
                                    {
                                        userLists[ev.ChannelId].Add(ev.FromUserId);
                                        OnUserConnectEvent?.Invoke(new MultiplexEvent(ev.FromUserId, Encoding.UTF8.GetString(info), new MultiplexData(data), ev.ChannelId));
                                    }
                                    else
                                    {
                                        userLists[ev.ChannelId].Remove(ev.FromUserId);
                                        OnUserDisconnectEvent?.Invoke(new MultiplexEvent(ev.FromUserId, Encoding.UTF8.GetString(info), new MultiplexData(data), ev.ChannelId));
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (ev.Error != MultiplexErrors.NoEvent)
                        {
                            Multiplex.c_disconnect(multiplex, 2500);
                            OnError?.Invoke(ev.Error);
                            throw new MultiplexException("An error occurred: " + ev.Error);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (multiplex.ToInt64() != 0)
                {
                    for (int i = 0; i < sendQueue.Count; i++)
                    {
                        MultiplexEvent sev;
                        if (sendQueue.TryDequeue(out sev))
                        {
                            byte[] infoAsBytes = Encoding.UTF8.GetBytes(sev.Info);
                            Multiplex.c_send(multiplex, sev.Data.bytes, (uint)sev.Data.bytes.Length, infoAsBytes, (uint)infoAsBytes.Length, sev.Channel, (int)sev.Flags);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Multiplex.c_destroy(multiplex);
                MTDebug.LogError(e);
            }
        }

        public static void Send(MultiplexEvent ev)
        {
            sendQueue.Enqueue(ev);
        }

        public void Tick()
        {
            sendratedelta += Time.unscaledDeltaTime;
            if (sendratedelta > (float)sendrate / 1000)
            {
                sendratedelta = 0f;
                NetworkTick?.Invoke();
            }
            Process();
        }
    }

    [Serializable]
    public class MultiplexException : Exception
    {
        public MultiplexException() { }
        public MultiplexException(string message) : base(message) { }
        public MultiplexException(string message, Exception inner) : base(message, inner) { }
        protected MultiplexException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public struct MultiplexEvent
    {
        internal MultiplexEvent(ulong user, string info, MultiplexData data, uint channel, MultiplexSendFlags flags = 0)
        {
            User = user;
            Info = info;
            Data = data;
            Channel = channel;
            Flags = flags;
        }

        public MultiplexEvent(string info, MultiplexData data, uint channel, MultiplexSendFlags flags = 0)
        {
            User = 0;
            Info = info;
            Data = data;
            Channel = channel;
            Flags = flags;
        }

        public ulong User;
        public string Info;
        public MultiplexData Data;
        public uint Channel;
        public MultiplexSendFlags Flags;
    }

    public struct MultiplexData
    {
        public MultiplexData(byte[] data)
        {
            bytes = data;
        }
        public byte[] bytes;
        public string text => Encoding.UTF8.GetString(bytes);
    }

    public enum MultiplexSendFlags
    {
        MT_SEND_RELIABLE = 1 << 0,
        MT_NO_FLUSH = 1 << 1
    };

    public enum MultiplexErrors
    {
        None = 0,
        ENet,
        NoEvent,
        FailedRelay
    };
}
