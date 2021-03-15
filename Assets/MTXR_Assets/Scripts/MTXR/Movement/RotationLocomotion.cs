using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace MTXR.Player.Movement
{
    // TODO: Any playerprefs or enum angle things are temporary until options exist.
    // TODO: doesn't work properly yet, and also won't work until MTVRTrackingSpace accounts for rotation
    public class RotationLocomotion : Locomotion
    {
        // Preferred style of rotation.
        public RotationStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                ApplySpeedFromStyle();
                _style = value;
            }
        }

        private RotationStyle _style;
        // Increment or speed applied to rotation input.
        public float RotationSpeed = 45f;
        // Input action set.
        private RotateActions _actions;

        private void Start()
        {
            _actions = new RotateActions();
            _actions.Enable();
            Style = (RotationStyle)PlayerPrefs.GetInt("rotationStylePreference", (int)RotationStyle.Snap45);
        }

        private void OnDestroy()
        {
            _actions.Disable();
            _actions.Dispose();
        }

        private void Update()
        {
            InputAction rotationAction = _actions.Rotation.Rotate;
            InputAction styleChangeAction = _actions.Rotation.ChangeStyle;
            if (styleChangeAction.triggered)
            {
                // This nasty little line just increments and wraps through the RotationStyles.
                Style = (RotationStyle)(((int)Style + 1) % Enum.GetNames(typeof(RotationStyle)).Length);
                PlayerPrefs.SetInt("rotationStylePreference", (int)Style);
            }

            switch (Style)
            {
                case RotationStyle.Snap45:
                case RotationStyle.Snap90:
                    if (rotationAction.triggered)
                    {
                        Player.transform.localRotation *= Quaternion.AngleAxis(Mathf.Sign(rotationAction.ReadValue<Vector2>().x) * RotationSpeed, Player.transform.up);
                    }
                    break;
                case RotationStyle.Continuous180:
                case RotationStyle.Continuous360:
                    Player.transform.localRotation *= Quaternion.AngleAxis(rotationAction.ReadValue<Vector2>().x * RotationSpeed * Time.deltaTime, Player.transform.up);
                    break;
            }
        }

        // TODO: Temporary until real options menus are a thing
        private void ApplySpeedFromStyle()
        {
            switch (_style)
            {
                case RotationStyle.Snap45:
                    RotationSpeed = 45f;
                    break;
                case RotationStyle.Snap90:
                    RotationSpeed = 90f;
                    break;
                case RotationStyle.Continuous180:
                    RotationSpeed = 180f;
                    break;
                case RotationStyle.Continuous360:
                    RotationSpeed = 360f;
                    break;
            }
        }

        public enum RotationStyle
        {
            Snap45,
            Snap90,
            Continuous180,
            Continuous360
        }
    }
}
