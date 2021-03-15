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
    public class MultiplexManager : IInitializable, ITickable
    {
        //sendrate
        public const int sendRate = 30;
        private float _sendRateDelta;

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

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // MT
        // private readonly string hostAddress = "104.37.189.85";
        // private readonly int hostPort = 3000;
        // SOUP
        private readonly string hostAddress = "108.230.44.131";
        private readonly int hostPort = 3000;
        // ;>
//#else
        //private readonly string hostAddress = "0.0.0.0";
        //private readonly int hostPort = 3000;
//#endif

        public void Initialize()
        {
            if (!Setup(hostAddress)) 
            {
                return;
            }

            OnSetup += () =>
            {
                multiplex.BindChannel(1, 1);
            };

            OnError += (error) =>
            {
                Setup(hostAddress);
            };

            OnDisconnect += () =>
            {
                Setup(hostAddress);
            };

            OnEvent += (MultiplexPacket ev) =>
            {
                if (ev.Info.text == "chat")
                {
                    MTDebug.Log($"<color=#F40496>[Multiplex Chat]</color> <b>{ev.User}</b>: \"{ev.Data.text}\"");
                }
            };
        }

        public bool Setup(string host)
        {
            try {
                MTDebug.Log("Starting Multiplex");
                multiplex = new MultiplexClient();
                multiplex.Connect(host, hostPort);
                return true;
            }
            catch (Exception e) 
            {
                if (e.GetType() == typeof(MultiplexException)) 
                {
                    MTDebug.LogError(e);
                }
                else 
                {
                    Debug.LogError(e);    
                }
                return false;
            }
            
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
            _sendRateDelta += Time.unscaledDeltaTime;
            if (_sendRateDelta > (float)sendRate / 1000)
            {
                _sendRateDelta = 0f;
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
