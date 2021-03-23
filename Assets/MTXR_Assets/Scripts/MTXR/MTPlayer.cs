using System.Collections.Generic;
using UnityEngine;
using MTXR.Player.Movement;
using UnityEngine.AddressableAssets;
using Megatowel.NetObject;
using FMODUnity;

namespace MTXR.Player
{
    [DisallowMultipleComponent]
    public class MTPlayer : NetBehaviour
    {
        /// <summary> 
        /// Static reference to our local player.
        /// </summary>
        public static MTPlayer LocalPlayer;

        /// <summary> 
        /// Reference to this player's head.
        /// </summary>
        public MTHead Head;

        /// <summary> 
        /// Reference to this player's left hand.
        /// </summary>
        public GameObject LeftHand;

        /// <summary> 
        /// Reference to this player's right hand.
        /// </summary>
        public GameObject RightHand;

        /// <summary> 
        /// Reference to this player's current locomotion style.
        /// </summary>
        /// NOTE: This should be replicated in some way, as the current locomotion will have networked parts.
        [HideInInspector]
        public List<Locomotion> Locomotions;

        private void Start()
        {
            SetupPlayer();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateLocal()
        {
            NetManager.SpawnNetAddressable("MTXR_Player");
        }

        /// <summary> 
        /// Configures a player depending on if they are a network player or not.
        /// </summary>
        private void SetupPlayer()
        {
            gameObject.name = $"{netView.Authority} (Player)";
            if (netView.authorityStatus != AuthorityStatus.RemoteAuthority && LocalPlayer == null)
            {
                gameObject.name += " (LOCAL)";
                // TODO: Locomotion networking.
                Locomotions.Add(gameObject.AddComponent<TeleportLocomotion>());
                Locomotions.Add(gameObject.AddComponent<RotationLocomotion>());
                foreach (Locomotion loco in Locomotions)
                {
                    loco.Player = this;
                }

                LocalPlayer = this;

                // Hide head locally
                foreach (MeshRenderer mesh in Head.Model.GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
            else
            {
                gameObject.name += " (REMOTE)";
                // Disable local things the remote player shouldn't have
                foreach (StudioListener listener in GetComponentsInChildren<StudioListener>())
                {
                    listener.enabled = false;
                }
                foreach (RBFollower follower in GetComponentsInChildren<RBFollower>())
                {
                    follower.enabled = false;
                }
                foreach (UnityEngine.InputSystem.XR.TrackedPoseDriver follower in GetComponentsInChildren<UnityEngine.InputSystem.XR.TrackedPoseDriver>())
                {
                    follower.enabled = false;
                }
                Head.Camera.enabled = false;
            }
            Debug.Log(gameObject.name);
        }

        private void Update()
        {
            if (netView.IsOwned)
            {
                netView.EditField(1, transform.position);
                netView.EditField(2, transform.rotation);
                netView.EditField(3, Head.transform.position);
                netView.EditField(4, Head.transform.rotation);
                netView.EditField(5, LeftHand.transform.position);
                netView.EditField(6, LeftHand.transform.rotation);
                netView.EditField(7, RightHand.transform.position);
                netView.EditField(8, RightHand.transform.rotation);
                netView.Submit();
            }
            else if (netView.Authority != 0)
            {
                transform.position = Vector3.Lerp(transform.position, netView.GetField<Vector3>(1), Time.deltaTime * 20);
                transform.rotation = Quaternion.Lerp(transform.rotation, netView.GetField<Quaternion>(2), Time.deltaTime * 20);
                Head.transform.position = Vector3.Lerp(Head.transform.position, netView.GetField<Vector3>(3), Time.deltaTime * 20);
                Head.transform.rotation = Quaternion.Lerp(Head.transform.rotation, netView.GetField<Quaternion>(4), Time.deltaTime * 20);
                LeftHand.transform.position = Vector3.Lerp(LeftHand.transform.position, netView.GetField<Vector3>(5), Time.deltaTime * 20);
                LeftHand.transform.rotation = Quaternion.Lerp(LeftHand.transform.rotation, netView.GetField<Quaternion>(6), Time.deltaTime * 20);
                RightHand.transform.position = Vector3.Lerp(RightHand.transform.position, netView.GetField<Vector3>(7), Time.deltaTime * 20);
                RightHand.transform.rotation = Quaternion.Lerp(RightHand.transform.rotation, netView.GetField<Quaternion>(8), Time.deltaTime * 20);
            }
        }
    }
}