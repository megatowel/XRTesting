using System;
using System.Text;
using System.Runtime.InteropServices;
using Megatowel.Multiplex;

namespace Megatowel.Multiplex
{
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
        public MultiplexEventType EventType;
        public ulong FromUserId;
        public uint ChannelId;
        public ulong InstanceId;
        public byte[] Data;
        public byte[] Info;
        public ulong[] UserIds;
        public MultiplexErrors Error;
        public int EnetError;
    };
    class MultiplexClient {
        private IntPtr c_instance = MultiplexWrapper.c_make_client();
        public bool connected = false;

        ~MultiplexClient() {
            MultiplexWrapper.c_destroy(c_instance);
        }

        public void Connect(string host, int port) {
            int result = MultiplexWrapper.c_setup(c_instance, Encoding.UTF8.GetBytes(host), port);
            if (result != 0) {
                throw new MultiplexException();
            }
            connected = true;
        }
        public int BindChannel(uint channel, ulong instanceId) {
            return MultiplexWrapper.c_bind_channel(c_instance, channel, instanceId);
        }
        public int Send(byte[] data, byte[] info, uint channel, MultiplexSendFlags flags) {
            return MultiplexWrapper.c_send(c_instance, data, (uint)data.Length, info, (uint)info.Length, channel, (int)flags);
        }

        public MultiplexEvent ProcessEvent(uint timeout) {
            MultiplexWrapperEvent ev = MultiplexWrapper.c_process_event(c_instance, timeout);
            if (ev.EventType == MultiplexEventType.Disconnected)
                connected = false;
            byte[] data = new byte[(int)ev.DataSize];
            byte[] info = new byte[(int)ev.InfoSize];
            byte[] userIdsBytes = new byte[(int)(ev.UserIdsSize*8)];
            ulong[] userIds = new ulong[(int)ev.UserIdsSize];
            if (ev.DataSize != 0)
            {
                Marshal.Copy(ev.Data, data, 0, (int)ev.DataSize);
            }
            if (ev.InfoSize != 0)
            {
                Marshal.Copy(ev.Info, info, 0, (int)ev.InfoSize);
            }
            if (ev.UserIdsSize != 0)
            {
                Marshal.Copy(ev.UserIds, userIdsBytes, 0, (int)(ev.UserIdsSize*8));
                for (int i = 0; i < ev.UserIdsSize; i++) {
                    userIds[i] = BitConverter.ToUInt64(userIdsBytes, i*8);
                }
            }
            return new MultiplexEvent {
                EventType = ev.EventType,
                FromUserId = ev.FromUserId,
                ChannelId = ev.ChannelId,
                InstanceId = ev.InstanceId,
                Data = data,
                Info = info,
                UserIds = userIds,
                Error = ev.Error,
                EnetError = ev.EnetError
            };
        }
        public int Disconnect(uint timeout) {
            connected = false;
            return MultiplexWrapper.c_disconnect(c_instance, timeout);
        }
    }
}