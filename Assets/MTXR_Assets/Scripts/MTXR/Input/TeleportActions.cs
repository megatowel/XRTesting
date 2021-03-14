// GENERATED AUTOMATICALLY FROM 'Assets/Input/TeleportActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @TeleportActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @TeleportActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TeleportActions"",
    ""maps"": [
        {
            ""name"": ""Teleportation"",
            ""id"": ""3d1022ea-5680-436e-8108-abd82457f2cb"",
            ""actions"": [
                {
                    ""name"": ""Teleport"",
                    ""type"": ""Button"",
                    ""id"": ""41801601-fecc-49e0-bdfe-92227df54b51"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9a9a8e74-22d7-4db3-8f29-5fc49c417b92"",
                    ""path"": ""<OculusTouchController>/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Teleport"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e21f4e5c-f225-477d-966c-62b3b95fac2a"",
                    ""path"": ""<WMRSpatialController>/touchpadClicked"",
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
        // Teleportation
        m_Teleportation = asset.FindActionMap("Teleportation", throwIfNotFound: true);
        m_Teleportation_Teleport = m_Teleportation.FindAction("Teleport", throwIfNotFound: true);
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

    // Teleportation
    private readonly InputActionMap m_Teleportation;
    private ITeleportationActions m_TeleportationActionsCallbackInterface;
    private readonly InputAction m_Teleportation_Teleport;
    public struct TeleportationActions
    {
        private @TeleportActions m_Wrapper;
        public TeleportationActions(@TeleportActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Teleport => m_Wrapper.m_Teleportation_Teleport;
        public InputActionMap Get() { return m_Wrapper.m_Teleportation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TeleportationActions set) { return set.Get(); }
        public void SetCallbacks(ITeleportationActions instance)
        {
            if (m_Wrapper.m_TeleportationActionsCallbackInterface != null)
            {
                @Teleport.started -= m_Wrapper.m_TeleportationActionsCallbackInterface.OnTeleport;
                @Teleport.performed -= m_Wrapper.m_TeleportationActionsCallbackInterface.OnTeleport;
                @Teleport.canceled -= m_Wrapper.m_TeleportationActionsCallbackInterface.OnTeleport;
            }
            m_Wrapper.m_TeleportationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Teleport.started += instance.OnTeleport;
                @Teleport.performed += instance.OnTeleport;
                @Teleport.canceled += instance.OnTeleport;
            }
        }
    }
    public TeleportationActions @Teleportation => new TeleportationActions(this);
    public interface ITeleportationActions
    {
        void OnTeleport(InputAction.CallbackContext context);
    }
}
