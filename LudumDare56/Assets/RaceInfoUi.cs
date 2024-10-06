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
    
    public void SetPlacing(int newPlacing)
    {
        string placingString = newPlacing.ToString();

        switch (newPlacing)
        {
            case 1:
                placingString = $"1<i><size={placingSuffixSize}>st</i></size>";
                break;
            case 2:
                placingString = $"2<i><size={placingSuffixSize}>nd</i></size>";
                break;
            case 3:
                placingString = $"3<i><size={placingSuffixSize}>rd</i></size>";
                break;
            case 4:
                placingString = $"4<i><size={placingSuffixSize}>th</i></size>";
                break;
        }
        
        placing.text = placingString;
    }
    
    public void SetCurrentLap(int lapNumber)
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
}
