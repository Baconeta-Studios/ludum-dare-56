using _Scripts.Managers;
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

    private void Start()
    {
        SetTotalLaps(RaceManager.Instance.TotalLaps);
    }
    
    public void SetPlayerPlacement(int newPlacing)
    {
        placing.text = StringUtils.FormatPlacing(newPlacing, placingSuffixSize);
    }
    
    public void SetCurrentPlayerLap(int lapNumber)
    {
        currentLap.text = Mathf.Clamp(lapNumber, 1, RaceManager.Instance.TotalLaps).ToString();
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
        lapTime.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(timeSeconds);
    }
    
    public void SetBestTime(float timeSeconds)
    {
        bestLapTime.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(timeSeconds);
    }
    
    public void SetRaceTime(float timeSeconds)
    {
        raceTime.text = StringUtils.ConvertFloatToMinutesSecondsMilliseconds(timeSeconds);
    }
    
    #endregion
    
    private void OnEnable()
    {
        GameTimeManager.OnRaceTimeChanged += SetRaceTime;
        GameTimeManager.OnLapTimeChanged += SetLapTime;
        GameTimeManager.OnFastestLapTimeChanged += SetBestTime;
        TrackPositionManager.OnPlayerRankingChanged += SetPlayerPlacement;
        TrackPositionManager.OnPlayerLapCompleted += SetCurrentPlayerLap;
    }

    private void OnDisable()
    {
        GameTimeManager.OnRaceTimeChanged -= SetRaceTime;
        GameTimeManager.OnLapTimeChanged -= SetLapTime;
        GameTimeManager.OnFastestLapTimeChanged -= SetBestTime;
        TrackPositionManager.OnPlayerRankingChanged -= SetPlayerPlacement;
        TrackPositionManager.OnPlayerLapCompleted += SetCurrentPlayerLap;
    }
}
