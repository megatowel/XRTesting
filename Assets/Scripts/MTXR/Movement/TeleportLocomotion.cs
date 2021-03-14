// TODO: When networking has been implemented, make this only perform visuals if remote
// TODO: Right now it just craps out objects, clean that up sometime
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


namespace MTXR.Player.Movement
{
    public class TeleportLocomotion : Locomotion
    {
        // TODO: Make some of these literals settings in some sort of ScriptableObject
        // Maximum distance that a teleport will be able to travel.
        public float MaxTeleportDistance = 6f;

        // Max normal angle that is considered a valid teleport destination, in degrees.
        public float MaxTeleportSteepness = 45f;

        // Mask of Physics layers that the ray is allowed to hit.
        public LayerMask ValidTeleportLayers;

        // The beginning position and angle of the ray.
        public Transform Origin;

        // Current state flags.
        private TeleportState _state;
        // The last cast's hit.
        private RaycastHit _teleportCastHit;
        // Reference to LineRenderer.
        private LineRenderer _teleportLine;
        // Reference to Teleport Marker object.
        private GameObject _teleportMarker;
        // Input action set.
        private TeleportActions _actions;
        // The device that is currently in control.
        private InputDevice _device;
        // Is the device that is in control left handed, right handed, or neither?
        private string _handedness;
        

        private void Start()
        {
            ValidTeleportLayers = LayerMask.GetMask("Default");
        }

        private void OnEnable()
        {
            _teleportLine = gameObject.AddComponent<LineRenderer>();
            _teleportLine.positionCount = 20;
            _teleportLine.widthMultiplier = 0.1f;
            _teleportLine.material = new Material(Shader.Find("MTXR/TeleportBeam"));
            _teleportLine.textureMode = LineTextureMode.Tile;
            _teleportLine.enabled = false;

            _teleportMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _teleportMarker.layer = LayerMask.NameToLayer("Ignore Raycast"); 
            Destroy(_teleportMarker.GetComponent<Collider>());
            _teleportMarker.GetComponent<Renderer>().material = _teleportLine.material;
            _teleportMarker.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);
            _teleportMarker.SetActive(false);


            _actions = new TeleportActions();
            _actions.Enable();
            _actions.Teleportation.Teleport.started += StartTeleport;
            _actions.Teleportation.Teleport.canceled += FinishTeleport;
        }

        private void OnDisable()
        {
            _actions.Teleportation.Teleport.started -= StartTeleport;
            _actions.Teleportation.Teleport.canceled -= FinishTeleport;
            _actions.Disable();
            _actions.Dispose();

            Destroy(_teleportMarker);
            Destroy(_teleportLine);
        }

        private void StartTeleport(InputAction.CallbackContext obj)
        {
            _device = obj.control.device;
            // Get whether the device that fired this is a left handed or right handed device, or neither.
            _handedness = _device.usages.First((usage) =>
                {
                    return usage == CommonUsages.LeftHand || usage == CommonUsages.RightHand;
                }
            );

            // Change the origin of the teleportation ray depending on device handedness characteristic.
            switch (_handedness)
            {
                case "LeftHand":
                    Origin = Player.LeftHand.transform;
                    break;
                case "RightHand":
                    Origin = Player.RightHand.transform;
                    break;
                default:
                    Origin = Player.Head.transform;
                    break;
            }

            // Start casting the ray.
            _state |= TeleportState.Casting;
            _teleportLine.enabled = true;
        }

        private void FinishTeleport(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            // Stop casting the ray.
            _state &= ~TeleportState.Casting;

            _teleportLine.enabled = false;
            _teleportMarker.SetActive(false);

            // If the new position is valid, teleport there and we're done!
            if ((_state & TeleportState.Valid) != 0)
            {
                Debug.Log(_device.name);
                ((XRControllerWithRumble)_device).SendImpulse(1f, 500f);
                
                
                DoTeleport();
            }
        }

        // Warp to the latest ray hit point.
        private void DoTeleport()
        {
            // The valid check isn't in here by design.
            Player.transform.position = _teleportCastHit.point;
        }

        // Update the ray and line every frame, and check if the chosen position is valid.
        private void Update()
        {
            if ((_state & TeleportState.Casting) != 0)
            {
                Vector3 acceleration = CalculateParabolaAcceleration();
                Vector3[] points;
                bool contacted = CalculateParabolaPoints(acceleration, out _teleportCastHit, out points, _teleportLine.positionCount);

                // Check our contact point's validity.
                if (contacted)
                {
                    // Is the angle of our hit too steep?
                    if (Vector3.Angle(_teleportCastHit.normal, Player.transform.up) <= MaxTeleportSteepness)
                    {
                        // A valid hit has been made.
                        _state |= TeleportState.Valid;
                    }
                    else
                    {
                        // A hit has been made, but it is too steep in normal angle to be valid.
                        _state &= ~TeleportState.Valid;
                    }
                }
                else
                {
                    // Nothing was hit.
                    _state &= ~TeleportState.Valid;
                }

                // Change the material's color depending on hit validity.
                // TODO: This will not be needed eventually.
                if ((_state & TeleportState.Valid) != 0)
                {
                    _teleportLine.material.EnableKeyword("_ISVALID");
                    _teleportMarker.SetActive(true);
                }
                else
                {
                    _teleportLine.material.DisableKeyword("_ISVALID");
                    _teleportMarker.SetActive(false);
                }



                for (int i = 0; i < points.Length; ++i)
                {
                    _teleportLine.SetPosition(i, points[i]);
                }

                _teleportMarker.transform.position = _teleportCastHit.point;
                _teleportMarker.transform.localRotation = Quaternion.LookRotation(_teleportCastHit.normal, _teleportMarker.transform.up);
            }
        }

        private static Vector3 CalculateParabola(Vector3 start, Vector3 startVelocity, Vector3 acceleration, float time)
        {
            return start + startVelocity * time + 0.5f * acceleration * time * time;
        }

        private static Vector3 CalculateParabolaDerivative(Vector3 startVelocity, Vector3 direction, float time)
        {
            return startVelocity + direction * time;
        }

        private bool CalculateParabolaPoints(Vector3 acceleration, out RaycastHit hit, out Vector3[] points, int numSegments = 20)
        {
            float segmentLength = 20f / numSegments;
            bool result = false;
            hit = default(RaycastHit);
            Vector3[] pointsResult = new Vector3[numSegments];

            Vector3 forward = Origin.forward;
            Vector3 position = Origin.position;
            Vector3 prevPoint = position;

            pointsResult[0] = position;

            Ray ray = new Ray(position, base.transform.forward);
            float distance = 0f;
            float time = 0f;
            for (int i = 1; i < pointsResult.Length; ++i)
            {
                time += segmentLength / CalculateParabolaDerivative(forward, acceleration, time).magnitude;
                Vector3 parabola = CalculateParabola(position, forward, acceleration, time);
                Vector3 direction = parabola - prevPoint;
                float magnitude = Vector3.Project(direction, base.transform.forward).magnitude;
                distance += magnitude;

                if (distance >= MaxTeleportDistance)
                {
                    float newTime = time;
                    for (int j = i; j < pointsResult.Length; ++j)
                    {
                        pointsResult[j] = parabola;
                    }
                    break;
                }

                ray = new Ray(prevPoint, direction.normalized);
                if (this.DoRaycast(ray, direction.magnitude, out hit))
                {
                    result = true;
                    pointsResult[i] = hit.point;
                    for (int j = i; j < pointsResult.Length; ++j)
                    {
                        pointsResult[j] = hit.point;
                    }
                    break;
                }

                pointsResult[i] = parabola;
                prevPoint = parabola;
            }
            points = pointsResult;
            return result;
        }

        private Vector3 CalculateParabolaAcceleration()
        {
            Vector3 projected = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
            Vector3 cross = Vector3.Cross(projected, Vector3.up).normalized;
            Vector3 direction = (Quaternion.AngleAxis(45f, cross) * projected).normalized;
            float dot = Vector3.Dot(direction, projected);
            float distance = MaxTeleportDistance / dot;
            Vector3 a = Vector3.Project(direction, Vector3.up);
            return -2f * a / distance;
        }

        private bool DoRaycast(Ray ray, float magnitude, out RaycastHit hit)
        {
            hit = default(RaycastHit);
            return Physics.Raycast(ray, out hit, magnitude, ValidTeleportLayers, QueryTriggerInteraction.Ignore);
        }

        [System.Flags]
        private enum TeleportState
        {
            None,
            Casting = 1 << 0,
            Valid = 1 << 1,
            // TODO: Not implemented
            Taper = 1 << 2,
        }
    }
}