using UnityEngine;

namespace Gameplay
{
    internal sealed class InputController : MonoBehaviour
    {
        [SerializeField]
        private float _speedDeltaPerTap = 32;
        [SerializeField]
        private MotorcycleController _motorcycle;

        private float _screenCenterXInPixels;

        private void Awake()
        {
            _screenCenterXInPixels = Screen.width * 0.5f;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                ChangeMotorcycleSpeed(Input.mousePosition.x);
            }
#else
            var touches = Input.touches;

            foreach (var touch in touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    ChangeMotorcycleSpeed(touch.position.x);
                }
            }
#endif
        }

        private void ChangeMotorcycleSpeed(float tapPositionX)
        {
            if (tapPositionX >= _screenCenterXInPixels)
            {
                _motorcycle.ChangeSpeed(_speedDeltaPerTap);
            }
            else
            {
                _motorcycle.ChangeSpeed(-_speedDeltaPerTap);
            }
        }
    }
}
