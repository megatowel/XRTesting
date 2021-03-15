using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MTXR.Player.Movement;

namespace MTXR.Player
{
    [DisallowMultipleComponent]
    public class MTPlayer : MonoBehaviour
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

        private PlayerActions _actions;

        private void OnEnable()
        {
            _actions.Enable();
        }

        private void OnDisable()
        {
            _actions.Disable();
        }

        private void Awake()
        {
            SetupPlayer(true);
        }

        private void Update()
        {
            LeftHand.transform.localPosition = _actions.Base.LeftHandPosition.ReadValue<Vector3>();
            LeftHand.transform.localRotation = _actions.Base.LeftHandRotation.ReadValue<Quaternion>();
            RightHand.transform.localPosition = _actions.Base.RightHandPosition.ReadValue<Vector3>();
            RightHand.transform.localRotation = _actions.Base.RightHandRotation.ReadValue<Quaternion>();
        }

        /// <summary> 
        /// Configures a player depending on if they are a network player or not.
        /// </summary>
        private void SetupPlayer(bool isLocal)
        {
            _actions = new PlayerActions();
            Locomotions.Add(gameObject.AddComponent<TeleportLocomotion>());
            Locomotions.Add(gameObject.AddComponent<RotationLocomotion>());
            foreach (Locomotion loco in Locomotions)
            {
                loco.Player = this;
            }

            if (isLocal && LocalPlayer == null)
            {
                LocalPlayer = this;
                // Hide head locally
                foreach (MeshRenderer mesh in Head.Model.GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
            else
            {
                // Disable remote player's trackers & Camera
                // TODO: RBFollower will not be used like this in the future.
                foreach (RBFollower follower in GetComponentsInChildren<RBFollower>())
                {
                    follower.enabled = false;
                }
                Head.Camera.enabled = false;
            }
        }
    }
}