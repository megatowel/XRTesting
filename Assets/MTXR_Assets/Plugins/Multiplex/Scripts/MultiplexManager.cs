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

        public static event Action<MultiplexPacket> OnEvent;
        public static event Action<MultiplexErrors> OnError;
        public static event Action<MultiplexPacket> OnUserConnectEvent;
        public static event Action<MultiplexPacket> OnUserDisconnectEvent;
        public static event Action OnSetup;
        public static event Action OnDisconnect;
        public static event Action NetworkTick;
        public static ulong SelfId;

        private MultiplexClient multiplex;
        private static ConcurrentQueue<MultiplexPacket> sendQueue = new ConcurrentQueue<MultiplexPacket>();
        private static ConcurrentDictionary<uint, List<ulong>> userLists = new ConcurrentDictionary<uint, List<ulong>>();

        // MT
        // private readonly string hostServer = "104.37.189.85";
        // private readonly int hostPort = 3000;
        // SOUP
        private readonly string hostServer = "108.230.44.131";
        private readonly int hostPort = 3000;
        // ;>

        public void Initialize()
        {
            Setup(hostServer);

            OnSetup += () =>
            {
                multiplex.BindChannel(1, 1);
            };

            OnError += (error) =>
            {
                Setup(hostServer);
            };

            OnDisconnect += () =>
            {
                Setup(hostServer);
            };

            OnEvent += (MultiplexPacket ev) =>
            {
                if (ev.Info.text == "chat")
                {
                    MTDebug.Log(ev.Data.text);
                }
            };
        }

        public void Setup(string host)
        {
            multiplex = new MultiplexClient();
            multiplex.Connect(host, hostPort);
            MTDebug.Log("Starting Multiplex");
        }

        public void Dispose()
        {
        }

        private void Process()
        {
            try
            {
                while (multiplex.connected)
                {
                    var ev = multiplex.ProcessEvent(0);
                    if (ev.Error == 0)
                    {
                        switch (ev.EventType)
                        {
                            case MultiplexEventType.UserSetup:
                                SelfId = ev.FromUserId;
                                OnSetup?.Invoke();
                                break;

                            case MultiplexEventType.UserMessage:
                                OnEvent?.Invoke(new MultiplexPacket(ev.FromUserId, new MultiplexData(ev.Info), new MultiplexData(ev.Data), ev.ChannelId));
                                break;

                            case MultiplexEventType.Disconnected:
                                OnDisconnect?.Invoke();
                                throw new MultiplexException("Lost connection to server.");

                            case MultiplexEventType.InstanceConnected:
                                userLists[ev.ChannelId] = new List<ulong>(ev.UserIds);
                                break;

                            case MultiplexEventType.InstanceUserUpdate:
                                if (ev.FromUserId != SelfId)
                                {
                                    if (ev.InstanceId != 0)
                                    {
                                        userLists[ev.ChannelId].Add(ev.FromUserId);
                                        OnUserConnectEvent?.Invoke(new MultiplexPacket(ev.FromUserId, new MultiplexData(ev.Info), new MultiplexData(ev.Data), ev.ChannelId));
                                    }
                                    else
                                    {
                                        userLists[ev.ChannelId].Remove(ev.FromUserId);
                                        OnUserDisconnectEvent?.Invoke(new MultiplexPacket(ev.FromUserId, new MultiplexData(ev.Info), new MultiplexData(ev.Data), ev.ChannelId));
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (ev.Error != MultiplexErrors.NoEvent)
                        {
                            multiplex.Disconnect(2500);
                            OnError?.Invoke(ev.Error);
                            throw new MultiplexException("An error occurred: " + ev.Error);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (multiplex.connected)
                {
                    for (int i = 0; i < sendQueue.Count; i++)
                    {
                        if (sendQueue.TryDequeue(out MultiplexPacket sev))
                        {
                            multiplex.Send(sev.Data.bytes, sev.Info.bytes, sev.Channel, sev.Flags);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                multiplex.Disconnect(100);
                MTDebug.LogError(e);
            }
        }

        public static void Send(MultiplexPacket ev)
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

    public struct MultiplexPacket
    {
        internal MultiplexPacket(ulong user, MultiplexData info, MultiplexData data, uint channel, MultiplexSendFlags flags = 0)
        {
            User = user;
            Info = info;
            Data = data;
            Channel = channel;
            Flags = flags;
        }

        public MultiplexPacket(MultiplexData info, MultiplexData data, uint channel, MultiplexSendFlags flags = 0)
        {
            User = 0;
            Info = info;
            Data = data;
            Channel = channel;
            Flags = flags;
        }

        public MultiplexPacket(string info, MultiplexData data, uint channel, MultiplexSendFlags flags = 0)
        {
            User = 0;
            Info = new MultiplexData(Encoding.UTF8.GetBytes(info));
            Data = data;
            Channel = channel;
            Flags = flags;
        }

        public ulong User;
        public MultiplexData Info;
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
}
