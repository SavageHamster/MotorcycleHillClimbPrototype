using DataLayer;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    internal sealed class MotorcycleController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rigidBody;
        [SerializeField]
        private List<WheelJoint2D> _driveWheels;
        [SerializeField]
        private float _maxSpeed = 850f;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private float _isUpsideDownTimeSec = float.MaxValue;

        private void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void Update()
        {
            if (transform.position.x > Data.Distance.Get())
            {
                Data.Distance.Set(transform.position.x);
            }

            CheckIsUpsideDown();
        }

        public void ResetMotorcycle()
        {
            foreach (var wheel in _driveWheels)
            {
                var motor = wheel.motor;
                motor.motorSpeed = 0;
                wheel.motor = motor;

                wheel.attachedRigidbody.velocity = Vector2.zero;
                wheel.attachedRigidbody.angularVelocity = 0;
            }

            transform.position = _startPosition;
            transform.rotation = _startRotation;

            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0;
        }

        public void ChangeSpeed(float speedDelta)
        {
            foreach (var wheel in _driveWheels)
            {
                var motor = wheel.motor;
                motor.motorSpeed = Mathf.Clamp(motor.motorSpeed + speedDelta, 0, _maxSpeed);
                wheel.motor = motor;
            }
        }

        private void CheckIsUpsideDown()
        {
            if (IsUpsideDown())
            {
                if (_isUpsideDownTimeSec == float.MaxValue)
                {
                    _isUpsideDownTimeSec = Time.time + SessionsManager.Instance.RestartDelaySec;
                }
            }
            else
            {
                _isUpsideDownTimeSec = float.MaxValue;
            }

            if (Time.time > _isUpsideDownTimeSec)
            {
                SessionsManager.Instance.RestartSession();
                _isUpsideDownTimeSec = float.MaxValue;
            }
        }

        private bool IsUpsideDown()
        {
            return Vector3.Dot(transform.up, Vector3.up) < 0f;
        }
    }
}
