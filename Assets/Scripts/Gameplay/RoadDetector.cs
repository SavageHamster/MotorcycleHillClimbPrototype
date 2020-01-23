using UnityEngine;

namespace Gameplay
{
    internal sealed class RoadDetector : MonoBehaviour
    {
        private const string RoadSegmentTag = "RoadSegment";

        [SerializeField]
        private float _raycastDistance = 2.5f;

        private readonly RaycastHit2D[] _raycastHit = new RaycastHit2D[1];

        public bool IsRoadClose { get; private set; }

        private void FixedUpdate()
        {
            if (Physics2D.RaycastNonAlloc(transform.position, -transform.up, _raycastHit, _raycastDistance) > 0 &&
                _raycastHit[0].collider != null)
            {
                IsRoadClose = _raycastHit[0].collider.tag == RoadSegmentTag;
            }
            else
            {
                IsRoadClose = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position - transform.up * _raycastDistance);
        }
#endif
    }
}
