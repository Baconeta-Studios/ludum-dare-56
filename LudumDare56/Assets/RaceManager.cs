using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;
    [SerializeField] private bool hasRaceStarted;
    [SerializeField] public bool HasRaceStarted => hasRaceStarted;
    [SerializeField] private bool hasRaceFinished;
    [SerializeField] public bool HasRaceFinished => hasRaceFinished;
    [SerializeField] private int currentLap;
    [SerializeField] private int totalLaps;
    public int TotalLaps => totalLaps;
    
    [SerializeField] private int countdownSeconds;
    
    public static event Action OnRaceCountdownStarting;
    public static event Action<int> OnRaceCountdownChanged;
    public static event Action<int> OnRaceStarted;
    public static event Action OnRaceCompleted;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RaceCountdown());
    }
    
    IEnumerator RaceCountdown()
    {
        OnRaceCountdownStarting?.Invoke();
        yield return new WaitForSeconds(1.5f);
        for (int i = countdownSeconds; i > 0; i--)
        {
            OnRaceCountdownChanged?.Invoke(i);
            yield return new WaitForSeconds(1f);
        }
        
        OnRaceCountdownChanged?.Invoke(0);
        
        OnRaceStarted?.Invoke(totalLaps);
        
        hasRaceStarted = true;
        
        yield return null;
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
