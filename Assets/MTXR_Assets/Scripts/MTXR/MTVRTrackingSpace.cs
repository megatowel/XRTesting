using UnityEngine;

namespace MTXR.Player
{
    [DisallowMultipleComponent]
    public class MTVRTrackingSpace : MonoBehaviour
    {
        public MTPlayer Player;

        [Range(0f, 1f)]
        public float HeightOffset;

        private Vector3 _previousTransformPosition;
        private Quaternion _previousHeadRotation;

        private void Update()
        {
            // Align the player's origin with their absolute tracked position.
            transform.localPosition = new Vector3(-Player.Head.transform.localPosition.x, HeightOffset, -Player.Head.transform.localPosition.z);
            Player.transform.localPosition += Player.transform.localRotation * new Vector3(-(transform.localPosition.x - _previousTransformPosition.x), 0, -(transform.localPosition.z - _previousTransformPosition.z));
            _previousTransformPosition = transform.localPosition;

            /*
            // TODO: doesn't actually work yet and i'm too dead tired to fix it
            // Align the player's rotation with their head's rotation on the Y axis.
            Player.transform.rotation *= _previousHeadRotation;
            Player.Head.transform.localRotation *= Quaternion.Euler(0, -Player.Head.transform.localRotation.eulerAngles.y, 0);
            _previousHeadRotation = Quaternion.Euler(0, Player.Head.transform.localRotation.eulerAngles.y, 0);
            */
        }
    }
}