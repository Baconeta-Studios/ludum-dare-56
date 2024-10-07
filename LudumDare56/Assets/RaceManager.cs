using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private RaceManager Instance;
    [SerializeField] private bool hasRaceStarted;
    [SerializeField] private bool hasRaceFinished;
    [SerializeField] private int currentLap;
    [SerializeField] private int totalLaps;
    [SerializeField] private int countdownSeconds;
    
    public static event Action<int> OnRaceCountdownChanged;
    public static event Action<int> OnRaceStarted;
    public static event Action OnRaceCompleted;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hasRaceStarted = true;
    }

    void OnEnable()
    {
        TrackPositionManager.OnPlayerLapCompleted += CheckForRaceEnded;
    }

    void OnDisable()
    {
        TrackPositionManager.OnPlayerLapCompleted -= CheckForRaceEnded;
    }

    void CheckForRaceEnded(int newCurrentLap)
    {
        currentLap = newCurrentLap;
        if (currentLap > totalLaps)
        {
            RaceEnded();
        }
    }

    void RaceEnded()
    {
        hasRaceFinished = true;
        TrackPositionManager.OnPlayerLapCompleted -= CheckForRaceEnded;
        OnRaceCompleted?.Invoke();
    }

}
