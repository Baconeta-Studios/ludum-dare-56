using TMPro;
using UnityEngine;

public class RaceInfoUi : MonoBehaviour
{
    [Header("Placing")]
    public TextMeshProUGUI placing;
    public float placingSuffixSize = 40f;
    
    [Header("Lap Info")]
    public TextMeshProUGUI currentLap;
    public TextMeshProUGUI totalLaps;
    
    [Header("Times")]
    public TextMeshProUGUI lapTime;
    public TextMeshProUGUI bestLapTime;
    public TextMeshProUGUI raceTime;

    public void SetPlayerPlacement(int newPlacing)
    {
        string placingString = newPlacing switch
        {
            1 => $"1<i><size={placingSuffixSize}>st</i></size>",
            2 => $"2<i><size={placingSuffixSize}>nd</i></size>",
            3 => $"3<i><size={placingSuffixSize}>rd</i></size>",
            _ => $"{newPlacing}<i><size={placingSuffixSize}>th</i></size>"
        };

        placing.text = placingString;
    }
    
    public void SetCurrentPlayerLap(int lapNumber)
    {
        currentLap.text = lapNumber.ToString();
    }
    
    public void SetTotalLaps(int lapCount)
    {
        totalLaps.text = lapCount.ToString();
    }
    
    #region Timings

    [ContextMenu("Test Lap Time")]
    public void TestLapTime()
    {
        SetLapTime(70.56f);
    }
    
    public void SetLapTime(float timeSeconds)
    {
        lapTime.text = FormatTimeSeconds(timeSeconds);
    }
    
    public void SetBestTime(float timeSeconds)
    {
        bestLapTime.text = FormatTimeSeconds(timeSeconds);
    }
    
    public void SetRaceTime(float timeSeconds)
    {
        raceTime.text = FormatTimeSeconds(timeSeconds);
    }

    public string FormatTimeSeconds(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60);
        int seconds = Mathf.FloorToInt(timeSeconds - minutes * 60);
        int milliseconds = Mathf.CeilToInt((timeSeconds * 100) % 100);
        
        return $"{minutes:D2} - {seconds:D2} - {milliseconds:D2}";
    }
    
    #endregion
    
    private void OnEnable()
    {
        GameTimeManager.OnRaceTimeChanged += SetRaceTime;
        GameTimeManager.OnLapTimeChanged += SetLapTime;
        GameTimeManager.OnFastestLapTimeChanged += SetBestTime;
        TrackPositionManager.OnPlayerPositionChanged += SetPlayerPlacement;
        TrackPositionManager.OnPlayerLapCompleted += SetCurrentPlayerLap;
    }

    private void OnDisable()
    {
        GameTimeManager.OnRaceTimeChanged -= SetRaceTime;
        GameTimeManager.OnLapTimeChanged -= SetLapTime;
        GameTimeManager.OnFastestLapTimeChanged -= SetBestTime;
        TrackPositionManager.OnPlayerPositionChanged -= SetPlayerPlacement;
        TrackPositionManager.OnPlayerLapCompleted += SetCurrentPlayerLap;
    }
}
