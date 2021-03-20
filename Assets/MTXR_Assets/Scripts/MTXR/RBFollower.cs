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

    public GameObject MeshObject;

    Vector3 m_CurrentPosition = Vector3.zero;
    Quaternion m_CurrentRotation = Quaternion.identity;
    Vector3 m_LastPosition = Vector3.zero;

    float m_VelocityMag = 0.0f;
    const float _handRBDistanceMaxMag = 0.05f;
    const float _handRBDistanceMaxAngle = 15f;
    float handMag = 0.0f;
    float handAngle = 0.0f;

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
        rb.maxAngularVelocity = 100f;
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
        // Position
        rb.velocity = (transform.parent.TransformPoint(newPosition) - rb.position) / Time.fixedDeltaTime;

        // Rotation
        Quaternion deltaRotation = (transform.parent.rotation * newRotation).normalized * Quaternion.Inverse(rb.rotation);
        Vector3 eulerRotation = new Vector3(
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.x)),
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.y)),
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.z))
        );

        rb.angularVelocity = eulerRotation / Time.fixedDeltaTime * Mathf.Deg2Rad;

        handMag = (transform.parent.TransformPoint(m_CurrentPosition) - rb.position).magnitude / _handRBDistanceMaxMag / m_VelocityMag;
        handAngle = Quaternion.Angle((transform.parent.rotation * m_CurrentRotation).normalized, rb.rotation) / _handRBDistanceMaxAngle;
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void FixedUpdate()
    {
        SetLocalTransform(m_CurrentPosition, m_CurrentRotation);
    }

    protected virtual void UpdateCallback()
    {
        // i swear i'm not this crazy
        
        MeshObject.transform.position = (handMag < 1.0f) ? Vector3.Lerp(transform.parent.TransformPoint(m_CurrentPosition), rb.position, (handMag < 0.5f) ? 0f : (handMag - 0.5f)*2) : rb.position;
        MeshObject.transform.rotation = (handAngle < 1.0f) ? Quaternion.Lerp((transform.parent.rotation * m_CurrentRotation).normalized, rb.rotation, (handAngle < 0.5f) ? 0f : (handAngle - 0.5f) * 2) : rb.rotation;
    }
    protected virtual void Update()
    {
        m_VelocityMag = (m_CurrentPosition - m_LastPosition).magnitude / Time.deltaTime;
        m_LastPosition = m_CurrentPosition;
    }
}