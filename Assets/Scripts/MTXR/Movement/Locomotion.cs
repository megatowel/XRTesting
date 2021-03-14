using System;
using UnityEngine;

namespace MTXR.Player.Movement
{
    // I was gonna do some cool stuff with this but now I don't know what should even actually be done here
    public abstract class Locomotion : MonoBehaviour
    {
        [HideInInspector]
        public MTPlayer Player;
    }
}
