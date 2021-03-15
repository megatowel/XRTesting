// GENERATED AUTOMATICALLY FROM 'Assets/MTXR_Assets/Scripts/MTXR/Input/RotateActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @RotateActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @RotateActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RotateActions"",
    ""maps"": [
        {
            ""name"": ""Rotation"",
            ""id"": ""91d6bfdc-5e2e-4aff-9a98-eae655ffc99e"",
            ""actions"": [
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""628e5275-60d7-463c-9015-9099a1f3546b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeStyle"",
                    ""type"": ""Button"",
                    ""id"": ""ff519dc4-5f7e-48a3-8db8-e1a919f46086"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6839f0c4-741d-4db3-84b3-8a6d0c676750"",
                    ""path"": ""<XRController>/thumbstickClicked"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeStyle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c836e5e-cb52-4973-a4ce-b9226d07064c"",
                    ""path"": ""<WMRSpatialController>/menu"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeStyle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e14b446b-66e6-472b-aea2-9815e212f144"",
                    ""path"": ""<XRController>/thumbstick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Rotation
        m_Rotation = asset.FindActionMap("Rotation", throwIfNotFound: true);
        m_Rotation_Rotate = m_Rotation.FindAction("Rotate", throwIfNotFound: true);
        m_Rotation_ChangeStyle = m_Rotation.FindAction("ChangeStyle", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Rotation
    private readonly InputActionMap m_Rotation;
    private IRotationActions m_RotationActionsCallbackInterface;
    private readonly InputAction m_Rotation_Rotate;
    private readonly InputAction m_Rotation_ChangeStyle;
    public struct RotationActions
    {
        private @RotateActions m_Wrapper;
        public RotationActions(@RotateActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotate => m_Wrapper.m_Rotation_Rotate;
        public InputAction @ChangeStyle => m_Wrapper.m_Rotation_ChangeStyle;
        public InputActionMap Get() { return m_Wrapper.m_Rotation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RotationActions set) { return set.Get(); }
        public void SetCallbacks(IRotationActions instance)
        {
            if (m_Wrapper.m_RotationActionsCallbackInterface != null)
            {
                @Rotate.started -= m_Wrapper.m_RotationActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_RotationActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_RotationActionsCallbackInterface.OnRotate;
                @ChangeStyle.started -= m_Wrapper.m_RotationActionsCallbackInterface.OnChangeStyle;
                @ChangeStyle.performed -= m_Wrapper.m_RotationActionsCallbackInterface.OnChangeStyle;
                @ChangeStyle.canceled -= m_Wrapper.m_RotationActionsCallbackInterface.OnChangeStyle;
            }
            m_Wrapper.m_RotationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @ChangeStyle.started += instance.OnChangeStyle;
                @ChangeStyle.performed += instance.OnChangeStyle;
                @ChangeStyle.canceled += instance.OnChangeStyle;
            }
        }
    }
    public RotationActions @Rotation => new RotationActions(this);
    public interface IRotationActions
    {
        void OnRotate(InputAction.CallbackContext context);
        void OnChangeStyle(InputAction.CallbackContext context);
    }
}
