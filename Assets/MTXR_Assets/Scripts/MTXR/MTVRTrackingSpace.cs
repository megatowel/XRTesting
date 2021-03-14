using UnityEngine;

namespace MTXR.Player
{
    public class MTVRTrackingSpace : MonoBehaviour
    {
        public MTPlayer Player;

        public Transform Root;

        [Range(0f, 1f)]
        public float HeightOffset;

        private Vector3 _previousTransformPosition = Vector3.zero;

        private void Update()
        {
            // Align the player's origin with their absolute tracked position.
            transform.localPosition = new Vector3(-Player.Head.transform.localPosition.x, HeightOffset, -Player.Head.transform.localPosition.z);
            Root.localPosition += new Vector3(-(transform.localPosition.x - _previousTransformPosition.x), 0, -(transform.localPosition.z - _previousTransformPosition.z));
            _previousTransformPosition = transform.localPosition;
        }

    }
}