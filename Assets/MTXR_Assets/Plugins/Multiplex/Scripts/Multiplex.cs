using System;
using System.Runtime.InteropServices;

namespace Megatowel.Multiplex
{
    public class Multiplex
    {
        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int init_enet();

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern IntPtr c_make_client();

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern IntPtr c_make_server();

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_setup(IntPtr multiplex, byte[] hostname, int port);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_destroy(IntPtr multiplex);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_disconnect(IntPtr multiplex, uint timeout);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_send(IntPtr multiplex, byte[] data, uint dataLength, byte[] info, uint infoLength, uint channel, int flags = 0);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_bind_channel(IntPtr multiplex, uint channel, ulong instance);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern int c_bind_channel_server(IntPtr multiplex, ulong userId, uint channel, ulong instance);

        [DllImport("libmultiplex", CharSet = CharSet.Unicode)]
        public static extern MultiplexEvent c_process_event(IntPtr multiplex, uint timeout);

        public struct MultiplexEvent
        {
            public MultiplexEventType EventType;
            public ulong FromUserId;
            public uint ChannelId;
            public ulong InstanceId;
            public IntPtr Data;
            public uint DataSize;
            public IntPtr Info;
            public uint InfoSize;
            public IntPtr UserIds;
            public uint UserIdsSize;
            public MultiplexErrors Error;
            public int EnetError;
        };

        public enum MultiplexActions
        {
            EditChannel,
            Server
        };

        public enum MultiplexSystemResponses
        {
            Message,
            UserSetup,
            InstanceConnected,
            InstanceUserJoin,
            InstanceUserLeave
        };

        public enum MultiplexEventType
        {
            Error = -1,
            UserMessage,
            UserSetup,
            Connected,
            InstanceConnected,
            Disconnected,
            InstanceUserUpdate,
            ServerCustom
        };
    }

}