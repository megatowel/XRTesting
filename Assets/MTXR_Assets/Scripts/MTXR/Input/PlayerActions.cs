// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
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
            ""name"": ""Base"",
            ""id"": ""0b541c4e-f844-48bd-a5b7-934c32a7eeeb"",
            ""actions"": [
                {
                    ""name"": ""LeftHandPosition"",
                    ""type"": ""Value"",
                    ""id"": ""9a38bedf-fc00-4c2d-82bb-8818557f1aa1"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""5e0c8cf8-ec4b-4790-b20a-f8bdcab21bdf"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightHandPosition"",
                    ""type"": ""Value"",
                    ""id"": ""33f6bc8a-6964-4c40-81ec-1c536b9399ed"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""cd3c9c47-6790-44dd-953d-e000378b74eb"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeadPosition"",
                    ""type"": ""Value"",
                    ""id"": ""4213fc5b-abf9-4963-b5cc-17e15398e0fd"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HeadRotation"",
                    ""type"": ""Value"",
                    ""id"": ""9a48194a-2cd8-4446-8b16-6504a8750db1"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftHandTracked"",
                    ""type"": ""Value"",
                    ""id"": ""4d8adc1d-a5a1-4321-b3d0-0284b1c21fc8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightHandTracked"",
                    ""type"": ""Value"",
                    ""id"": ""8f359040-929f-4efc-adcb-d15fcd87b3fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5d0b8381-5a65-40ff-80ca-0fd6e4e6ee31"",
                    ""path"": ""<XRController>{LeftHand}/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""afc1cb9d-0746-4cfb-864c-89aed17e9339"",
                    ""path"": ""<XRController>{RightHand}/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""804db6f9-4f37-44d0-b9ac-a62c06dacb6b"",
                    ""path"": ""<XRHMD>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeadPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83ff4c5d-ae83-43b0-ac3f-c71714101363"",
                    ""path"": ""<XRController>{LeftHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9dd21b44-38eb-4164-a32f-13924472971a"",
                    ""path"": ""<XRController>{RightHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8f865e5-45d7-433d-9e24-5a817410c0fb"",
                    ""path"": ""<XRHMD>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeadRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1bb5ec0d-7ece-4e06-b407-c55c993de290"",
                    ""path"": ""<XRController>{LeftHand}/isTracked"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftHandTracked"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd38bad5-41e0-46a9-9d27-6e02afdb7182"",
                    ""path"": ""<XRController>{RightHand}/isTracked"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHandTracked"",
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
        // Base
        m_Base = asset.FindActionMap("Base", throwIfNotFound: true);
        m_Base_LeftHandPosition = m_Base.FindAction("LeftHandPosition", throwIfNotFound: true);
        m_Base_LeftHandRotation = m_Base.FindAction("LeftHandRotation", throwIfNotFound: true);
        m_Base_RightHandPosition = m_Base.FindAction("RightHandPosition", throwIfNotFound: true);
        m_Base_RightHandRotation = m_Base.FindAction("RightHandRotation", throwIfNotFound: true);
        m_Base_HeadPosition = m_Base.FindAction("HeadPosition", throwIfNotFound: true);
        m_Base_HeadRotation = m_Base.FindAction("HeadRotation", throwIfNotFound: true);
        m_Base_LeftHandTracked = m_Base.FindAction("LeftHandTracked", throwIfNotFound: true);
        m_Base_RightHandTracked = m_Base.FindAction("RightHandTracked", throwIfNotFound: true);
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
        private @PlayerActions m_Wrapper;
        public DebugActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
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

    // Base
    private readonly InputActionMap m_Base;
    private IBaseActions m_BaseActionsCallbackInterface;
    private readonly InputAction m_Base_LeftHandPosition;
    private readonly InputAction m_Base_LeftHandRotation;
    private readonly InputAction m_Base_RightHandPosition;
    private readonly InputAction m_Base_RightHandRotation;
    private readonly InputAction m_Base_HeadPosition;
    private readonly InputAction m_Base_HeadRotation;
    private readonly InputAction m_Base_LeftHandTracked;
    private readonly InputAction m_Base_RightHandTracked;
    public struct BaseActions
    {
        private @PlayerActions m_Wrapper;
        public BaseActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftHandPosition => m_Wrapper.m_Base_LeftHandPosition;
        public InputAction @LeftHandRotation => m_Wrapper.m_Base_LeftHandRotation;
        public InputAction @RightHandPosition => m_Wrapper.m_Base_RightHandPosition;
        public InputAction @RightHandRotation => m_Wrapper.m_Base_RightHandRotation;
        public InputAction @HeadPosition => m_Wrapper.m_Base_HeadPosition;
        public InputAction @HeadRotation => m_Wrapper.m_Base_HeadRotation;
        public InputAction @LeftHandTracked => m_Wrapper.m_Base_LeftHandTracked;
        public InputAction @RightHandTracked => m_Wrapper.m_Base_RightHandTracked;
        public InputActionMap Get() { return m_Wrapper.m_Base; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BaseActions set) { return set.Get(); }
        public void SetCallbacks(IBaseActions instance)
        {
            if (m_Wrapper.m_BaseActionsCallbackInterface != null)
            {
                @LeftHandPosition.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandPosition.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandPosition.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandRotation.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandRotation;
                @LeftHandRotation.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandRotation;
                @LeftHandRotation.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandRotation;
                @RightHandPosition.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandPosition;
                @RightHandPosition.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandPosition;
                @RightHandPosition.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandPosition;
                @RightHandRotation.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandRotation;
                @RightHandRotation.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandRotation;
                @RightHandRotation.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandRotation;
                @HeadPosition.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadPosition;
                @HeadPosition.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadPosition;
                @HeadPosition.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadPosition;
                @HeadRotation.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadRotation;
                @HeadRotation.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadRotation;
                @HeadRotation.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnHeadRotation;
                @LeftHandTracked.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandTracked;
                @LeftHandTracked.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandTracked;
                @LeftHandTracked.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnLeftHandTracked;
                @RightHandTracked.started -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandTracked;
                @RightHandTracked.performed -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandTracked;
                @RightHandTracked.canceled -= m_Wrapper.m_BaseActionsCallbackInterface.OnRightHandTracked;
            }
            m_Wrapper.m_BaseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftHandPosition.started += instance.OnLeftHandPosition;
                @LeftHandPosition.performed += instance.OnLeftHandPosition;
                @LeftHandPosition.canceled += instance.OnLeftHandPosition;
                @LeftHandRotation.started += instance.OnLeftHandRotation;
                @LeftHandRotation.performed += instance.OnLeftHandRotation;
                @LeftHandRotation.canceled += instance.OnLeftHandRotation;
                @RightHandPosition.started += instance.OnRightHandPosition;
                @RightHandPosition.performed += instance.OnRightHandPosition;
                @RightHandPosition.canceled += instance.OnRightHandPosition;
                @RightHandRotation.started += instance.OnRightHandRotation;
                @RightHandRotation.performed += instance.OnRightHandRotation;
                @RightHandRotation.canceled += instance.OnRightHandRotation;
                @HeadPosition.started += instance.OnHeadPosition;
                @HeadPosition.performed += instance.OnHeadPosition;
                @HeadPosition.canceled += instance.OnHeadPosition;
                @HeadRotation.started += instance.OnHeadRotation;
                @HeadRotation.performed += instance.OnHeadRotation;
                @HeadRotation.canceled += instance.OnHeadRotation;
                @LeftHandTracked.started += instance.OnLeftHandTracked;
                @LeftHandTracked.performed += instance.OnLeftHandTracked;
                @LeftHandTracked.canceled += instance.OnLeftHandTracked;
                @RightHandTracked.started += instance.OnRightHandTracked;
                @RightHandTracked.performed += instance.OnRightHandTracked;
                @RightHandTracked.canceled += instance.OnRightHandTracked;
            }
        }
    }
    public BaseActions @Base => new BaseActions(this);
    public interface IDebugActions
    {
        void OnDebugAction(InputAction.CallbackContext context);
    }
    public interface IBaseActions
    {
        void OnLeftHandPosition(InputAction.CallbackContext context);
        void OnLeftHandRotation(InputAction.CallbackContext context);
        void OnRightHandPosition(InputAction.CallbackContext context);
        void OnRightHandRotation(InputAction.CallbackContext context);
        void OnHeadPosition(InputAction.CallbackContext context);
        void OnHeadRotation(InputAction.CallbackContext context);
        void OnLeftHandTracked(InputAction.CallbackContext context);
        void OnRightHandTracked(InputAction.CallbackContext context);
    }
}
