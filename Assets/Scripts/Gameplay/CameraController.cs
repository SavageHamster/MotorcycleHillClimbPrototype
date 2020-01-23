using UnityEngine;

namespace Gameplay
{
    internal sealed class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private float _initialCameraY;
        private float _initialCameraZ;
        private Camera _mainCamera;

        private void Awake()
        {
            _initialCameraY = transform.position.y;
            _initialCameraZ = transform.position.z;

            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(_target.position.x + _mainCamera.orthographicSize,
                _initialCameraY, _initialCameraZ);
        }
    }
}
