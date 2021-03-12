using Megatowel.Debugging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Megatowel.Multiplex
{
    public class MultiplexBehaviour : MonoBehaviour
    {
        public static ReadOnlyCollection<MultiplexBehaviour> All => _all.AsReadOnly();
        private static List<MultiplexBehaviour> _all = new List<MultiplexBehaviour>();


        public ulong Owner { get; private set; }

        // TODO: Implement this alongside instantiation
        public uint Channel { get; private set; }

        public ulong Self => MultiplexManager.SelfId;

        private byte LocalFieldIncrement = 0;

        public bool IsLocal => (Owner == Self);

        public MultiplexStream Stream = new MultiplexStream();

        private float time;

        public MultiplexBehaviour() {
            _all.Add(this);
            Channel = 1;
            Owner = Self;
            MultiplexManager.OnEvent += OnEvent;
            MultiplexManager.NetworkTick += MultiplexManager_NetworkTick;
            Debug.Log("lol");
        }

        private void OnEnable()
        {
            Debug.Log("lol2");
        }

        private void MultiplexManager_NetworkTick()
        {

            byte[] totalData = new byte[0];
            int arraySize = 0;
            byte[] data;

            // we getting funky with the queues and for loops
            for (int i = Stream.sending.Count; i > 0; i--)
            {
                while (true)
                {
                    if (Stream.sending.TryDequeue(out data))
                    {
                        Array.Resize(ref totalData, arraySize + data.Length);
                        Array.Copy(data, 0, totalData, arraySize, data.Length);
                        arraySize += data.Length;
                        break;
                    }
                }
            }
            MTDebug.Log(Stream.sending.Count);
            if (arraySize > 0)
                MultiplexManager.Send(new MultiplexPacket("game", new MultiplexData(totalData), 1U));
        }

        private void OnEvent(MultiplexPacket ev)
        {
            //if (ev.User != MultiplexManager.SelfId)
            //{
            if (ev.Info == "game")
            {
                MTDebug.Log("we gettin stuff");
                for (int i = 0; i < ev.Data.bytes.Length; i++)
                {
                    Stream.receiving.Enqueue(ev.Data.bytes[i]);
                }
            }
            //}
        }

        private void OnDisable()
        {
            MTDebug.Log("behaviour disable");
            MultiplexManager.OnEvent -= OnEvent;
            _all.Remove(this);
        }

        private void Reset()
        {

        }

    }

}
