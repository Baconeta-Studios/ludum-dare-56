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
    public int TotalLaps => totalLaps;
    
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
        StartCoroutine(RaceCountdown());
    }
    
    IEnumerator RaceCountdown()
    {
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
