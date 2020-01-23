using DataLayer;
using UnityEngine;

namespace UI
{
    internal sealed class GameScreen : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI _distanceLabel;
        [SerializeField]
        private GameObject _wheelieGameObject;

        private void Awake()
        {
            Data.Distance.Subscribe(OnDistanceChanged);
            Data.IsWheelie.Subscribe(OnIsWheelieChanged);
        }

        private void OnDestroy()
        {
            Data.Distance.Unsubscribe(OnDistanceChanged);
            Data.IsWheelie.Unsubscribe(OnIsWheelieChanged);
        }

        private void OnIsWheelieChanged()
        {
            _wheelieGameObject.SetActive(Data.IsWheelie.Get());
        }

        private void OnDistanceChanged()
        {
            _distanceLabel.text = ((int)(Data.Distance.Get())).ToString();
        }
    }
}
