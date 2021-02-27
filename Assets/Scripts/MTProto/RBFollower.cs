using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
[AddComponentMenu("XR/Tracked Rigidbody Pose Driver (New Input System)")]
[RequireComponent(typeof(Rigidbody))]
public class RBFollower : MonoBehaviour
{
    public enum TrackingType
    {
        RotationAndPosition,
        RotationOnly,
        PositionOnly
    }

    private Rigidbody rb;

    [SerializeField]
    TrackingType m_TrackingType;
    /// <summary>
    /// The tracking type being used by the tracked pose driver
    /// </summary>
    public TrackingType trackingType
    {
        get { return m_TrackingType; }
        set { m_TrackingType = value; }
    }

    [SerializeField]
    InputAction m_PositionAction;
    public InputAction positionAction
    {
        get { return m_PositionAction; }
        set
        {
            UnbindPosition();
            m_PositionAction = value;
            BindActions();
        }
    }

    [SerializeField]
    InputAction m_RotationAction;
    public InputAction rotationAction
    {
        get { return m_RotationAction; }
        set
        {
            UnbindRotation();
            m_RotationAction = value;
            BindActions();
        }
    }

    Vector3 m_CurrentPosition = Vector3.zero;
    Quaternion m_CurrentRotation = Quaternion.identity;
    bool m_RotationBound = false;
    bool m_PositionBound = false;

    void BindActions()
    {
        BindPosition();
        BindRotation();
    }

    void BindPosition()
    {
        if (!m_PositionBound && m_PositionAction != null)
        {
            m_PositionAction.Rename($"{gameObject.name} - TPD - Position");
            m_PositionAction.performed += OnPositionUpdate;
            m_PositionBound = true;
            m_PositionAction.Enable();
        }
    }

    void BindRotation()
    {
        if (!m_RotationBound && m_RotationAction != null)
        {
            m_RotationAction.Rename($"{gameObject.name} - TPD - Rotation");
            m_RotationAction.performed += OnRotationUpdate;
            m_RotationBound = true;
            m_RotationAction.Enable();
        }
    }

    void UnbindActions()
    {
        UnbindPosition();
        UnbindRotation();
    }

    void UnbindPosition()
    {
        if (m_PositionAction != null && m_PositionBound)
        {
            m_PositionAction.Disable();
            m_PositionAction.performed -= OnPositionUpdate;
            m_PositionBound = false;
        }
    }

    void UnbindRotation()
    {
        if (m_RotationAction != null && m_RotationBound)
        {
            m_RotationAction.Disable();
            m_RotationAction.performed -= OnRotationUpdate;
            m_RotationBound = false;
        }
    }

    void OnPositionUpdate(InputAction.CallbackContext context)
    {
        Debug.Assert(m_PositionBound);
        m_CurrentPosition = context.ReadValue<Vector3>();
    }

    void OnRotationUpdate(InputAction.CallbackContext context)
    {
        Debug.Assert(m_RotationBound);
        m_CurrentRotation = context.ReadValue<Quaternion>();
    }

    protected virtual void Awake()
    {
        rb = (Rigidbody)gameObject.GetComponent(typeof(Rigidbody));
    }

    protected void OnEnable()
    {
        InputSystem.onAfterUpdate += UpdateCallback;
        BindActions();
    }

    void OnDisable()
    {
        UnbindActions();
        InputSystem.onAfterUpdate -= UpdateCallback;
    }

    protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
    {
        
        rb.MovePosition(transform.parent.TransformPoint(newPosition));
        rb.MoveRotation((transform.parent.rotation * newRotation).normalized);
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void UpdateCallback()
    {
        SetLocalTransform(m_CurrentPosition, m_CurrentRotation);
    }
}