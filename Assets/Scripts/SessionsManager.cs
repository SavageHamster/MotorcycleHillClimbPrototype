using DataLayer;
using Gameplay;
using System;
using UnityEngine;

public sealed class SessionsManager : SingletonMonoBehaviour<SessionsManager>
{
    public event Action SessionRestarted;

    [SerializeField]
    private float _restartDelaySec = 4f;
    [SerializeField]
    private RoadGenerator _roadGenerator;
    [SerializeField]
    private MotorcycleController _motorcycleController;

    public float RestartDelaySec => _restartDelaySec;

    public void RestartSession()
    {
        Data.Reset();

        _motorcycleController.ResetMotorcycle();
        _roadGenerator.ResetRoad();

        SessionRestarted?.Invoke();
    }
}
