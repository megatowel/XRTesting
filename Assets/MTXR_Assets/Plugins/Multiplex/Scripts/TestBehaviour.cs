﻿using Megatowel.Debugging;
using System;

namespace Megatowel.Multiplex
{
    internal class TestBehaviour : MultiplexBehaviour
    {
        private void OnEnable()
        {
            MTDebug.Log("enabled");
        }

        private void OnDisable()
        {
            MTDebug.Log("disabled");
        }

        private void Update()
        {

            MTDebug.Log("serialize");
            Stream.SendNext(37);
            MTDebug.Log(Stream.ReceiveNext<int>());
            /*
            try
            {
                MTDebug.Log("receiving " + Stream.receiving.Count);
                MTDebug.Log(Stream.ReceiveNext<string>());
                //MTDebug.Log(Stream.ReceiveNext<int>());
            }
            catch (Exception e)
            {

            }*/
        }

        struct poop
        {
            string name;
            int epic;
        }
    }
}
