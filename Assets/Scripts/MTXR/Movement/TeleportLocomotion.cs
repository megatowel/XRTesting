// TODO: When networking has been implemented, make this only perform visuals if remote
// TODO: Right now it just craps out objects, clean that up sometime
using UnityEngine;

namespace MTXR.Player.Movement
{
    public class TeleportLocomotion : Locomotion
    {
        // Maximum distance that a teleport will be able to travel.
        public float MaxTeleportDistance = 6f;

        // Max normal angle that is considered a valid teleport destination, in degrees.
        public float MaxTeleportSteepness = 45f;

        // TODO: Make this work off of this origin point instead of just the player's LeftHand.
        // public Transform Origin;

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

        private static Vector3 CalculateParabola(Vector3 start, Vector3 startVelocity, Vector3 acceleration, float time)
        {
            return start + startVelocity * time + 0.5f * acceleration * time * time;
        }

        private static Vector3 CalculateParabolaDerivative(Vector3 startVelocity, Vector3 direction, float time)
        {
            return startVelocity + direction * time;
        }

        private bool CalculateParabolaPoints(Vector3 acceleration, out RaycastHit hit, out Vector3[] points, int numSegments = 20, float segmentLength = 1f)
        {
            bool result = false;
            hit = default(RaycastHit);
            Vector3[] pointsResult = new Vector3[numSegments];

            Vector3 forward = Player.LeftHand.transform.forward;
            Vector3 position = Player.LeftHand.transform.position;
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
                    break;
                }

                ray = new Ray(prevPoint, direction.normalized);
                if (this.DoRaycast(ray, direction.magnitude, out hit))
                {
                    result = true;
                    pointsResult[i] = hit.point;
                    for (int j = i; j < pointsResult.Length; ++j) {
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
            return Physics.Raycast(ray, out hit, magnitude, ~0, QueryTriggerInteraction.Ignore);
        }

        private void OnEnable()
        {
            _teleportLine = gameObject.AddComponent<LineRenderer>();
            _teleportLine.positionCount = 8;
            _teleportLine.widthMultiplier = 0.1f;
            _teleportLine.material = new Material(Shader.Find("Sprites/Default"));
            _teleportMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(_teleportMarker.GetComponent<CapsuleCollider>());
            _teleportLine.enabled = false;
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

        private void StartTeleport(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _state |= TeleportState.Casting;
            _teleportLine.enabled = true;
            // TODO: Uncomment this and the one in FinishTeleport when you want the teleport marker to be visible again. It's just a huge cylinder right now.
            //_teleportMarker.SetActive(true);
        }

        private void FinishTeleport(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _state &= ~TeleportState.Casting;
            //_teleportMarker.SetActive(false);
            _teleportLine.enabled = false;
            if ((_state & TeleportState.Valid) != 0)
            {
                DoTeleport();
            }
        }

        private void DoTeleport()
        {
            Player.transform.position = _teleportCastHit.point;
        }

        // Update is called once per frame
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
                    _teleportLine.material.color = _teleportMarker.GetComponent<MeshRenderer>().material.color = new Color(0.6499999f, 0.125f, 1f);
                }
                else
                {
                    _teleportLine.material.color = _teleportMarker.GetComponent<MeshRenderer>().material.color = Color.red;
                }

                for (int i = 0; i < points.Length; ++i) 
                {
                    _teleportLine.SetPosition(i, points[i]);
                }

                _teleportMarker.transform.position = _teleportCastHit.point;
                _teleportMarker.transform.rotation = Quaternion.Euler(_teleportCastHit.normal);
            }
        }

        [System.Flags]
        private enum TeleportState
        {
            None,
            Casting = 1 << 0,
            Valid = 1 << 1,
        }
    }
}