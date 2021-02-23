// GENERATED AUTOMATICALLY FROM 'Assets/Input/XRTestingActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @XRTestingActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @XRTestingActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""XRTestingActions"",
    ""maps"": [
        {
            ""name"": ""Debug"",
            ""id"": ""52fa52e9-0a96-4e8a-8ea6-9a6e969ddacf"",
            ""actions"": [
                {
                    ""name"": ""DebugAction"",
                    ""type"": ""Button"",
                    ""id"": ""3b4023e8-ce01-4dc2-8970-a2e20ec31e50"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""91ec0af3-c791-43c9-8c9a-f4355f5ec695"",
                    ""path"": ""<XRController>{RightHand}/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""XRTestPlayer"",
            ""id"": ""0b541c4e-f844-48bd-a5b7-934c32a7eeeb"",
            ""actions"": [
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""99c0e4c8-a3de-4d4f-9bb4-a02c3ae683fb"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Teleport"",
                    ""type"": ""Button"",
                    ""id"": ""fee6774a-afaf-42d7-b9ff-d34327bc3022"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""992a26c0-7d6b-49c4-92f4-344a78dc85f4"",
                    ""path"": ""<OculusTouchController>{RightHand}/thumbstick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b26622bf-54d8-4d3f-a688-9ffffce9c4ac"",
                    ""path"": ""<OculusTouchController>{RightHand}/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Teleport"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_DebugAction = m_Debug.FindAction("DebugAction", throwIfNotFound: true);
        // XRTestPlayer
        m_XRTestPlayer = asset.FindActionMap("XRTestPlayer", throwIfNotFound: true);
        m_XRTestPlayer_Rotate = m_XRTestPlayer.FindAction("Rotate", throwIfNotFound: true);
        m_XRTestPlayer_Teleport = m_XRTestPlayer.FindAction("Teleport", throwIfNotFound: true);
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

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_DebugAction;
    public struct DebugActions
    {
        private @XRTestingActions m_Wrapper;
        public DebugActions(@XRTestingActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @DebugAction => m_Wrapper.m_Debug_DebugAction;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @DebugAction.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugAction;
                @DebugAction.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugAction;
                @DebugAction.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugAction;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DebugAction.started += instance.OnDebugAction;
                @DebugAction.performed += instance.OnDebugAction;
                @DebugAction.canceled += instance.OnDebugAction;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);

    // XRTestPlayer
    private readonly InputActionMap m_XRTestPlayer;
    private IXRTestPlayerActions m_XRTestPlayerActionsCallbackInterface;
    private readonly InputAction m_XRTestPlayer_Rotate;
    private readonly InputAction m_XRTestPlayer_Teleport;
    public struct XRTestPlayerActions
    {
        private @XRTestingActions m_Wrapper;
        public XRTestPlayerActions(@XRTestingActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotate => m_Wrapper.m_XRTestPlayer_Rotate;
        public InputAction @Teleport => m_Wrapper.m_XRTestPlayer_Teleport;
        public InputActionMap Get() { return m_Wrapper.m_XRTestPlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(XRTestPlayerActions set) { return set.Get(); }
        public void SetCallbacks(IXRTestPlayerActions instance)
        {
            if (m_Wrapper.m_XRTestPlayerActionsCallbackInterface != null)
            {
                @Rotate.started -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnRotate;
                @Teleport.started -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnTeleport;
                @Teleport.performed -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnTeleport;
                @Teleport.canceled -= m_Wrapper.m_XRTestPlayerActionsCallbackInterface.OnTeleport;
            }
            m_Wrapper.m_XRTestPlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Teleport.started += instance.OnTeleport;
                @Teleport.performed += instance.OnTeleport;
                @Teleport.canceled += instance.OnTeleport;
            }
        }
    }
    public XRTestPlayerActions @XRTestPlayer => new XRTestPlayerActions(this);
    public interface IDebugActions
    {
        void OnDebugAction(InputAction.CallbackContext context);
    }
    public interface IXRTestPlayerActions
    {
        void OnRotate(InputAction.CallbackContext context);
        void OnTeleport(InputAction.CallbackContext context);
    }
}
