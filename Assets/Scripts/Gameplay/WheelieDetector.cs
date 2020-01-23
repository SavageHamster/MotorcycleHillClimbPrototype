using DataLayer;
using UnityEngine;

namespace Gameplay
{
    internal sealed class WheelieDetector : MonoBehaviour
    {
        [SerializeField]
        private RoadDetector _rearRoadDetector;
        [SerializeField]
        private RoadDetector _frontRoadDetector;

        private void Update()
        {
            Data.IsWheelie.Set(_rearRoadDetector.IsRoadClose && !_frontRoadDetector.IsRoadClose);
        }
    }
}
