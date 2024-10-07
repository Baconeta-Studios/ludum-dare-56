using System;
using System.Collections.Generic;
using _Scripts.Managers;
using TMPro;
using UnityEngine;

public class HighScoreTab : MonoBehaviour
{
    [Serializable]
    public enum HighScoreType
    {
        RaceTimes,
        LapTimes,
    }

    public HighScoreType currentlyShowing;
    
    public TextMeshProUGUI title;
    public string lapTimeTitle;
    public string raceTimeTitle;
    public Transform highScoreContainer;
    public List<HighScoreEntry> entries;

    void OnEnable()
    {
        switch (currentlyShowing)
        {
            case HighScoreType.RaceTimes:
                ShowTrackTimes();
                break;
            case HighScoreType.LapTimes:
                ShowLapTimes();
                break;
        }
    }
    
    [ContextMenu("Refresh Numbers")]
    public void UpdateNumbers()
    {
        entries.Clear();
        for (int i = 0; i < highScoreContainer.childCount; i++)
        {
            entries.Add(highScoreContainer.GetChild(i).GetComponent<HighScoreEntry>());
        }
        for(int i = 0; i < entries.Count; i++)
        {
            entries[i].SetNumber(i + 1);
        }
    }

    public void ShowTrackTimes()
    {
        ScoreServerManager.OnLapDataUpdate -= UpdateHighScores;
        ScoreServerManager.OnTrackDataUpdate += UpdateHighScores;
        currentlyShowing = HighScoreType.RaceTimes;
        // Update high scores from server's data.
        UpdateHighScores(ScoreServerManager.Instance.GetTrackData());
        title.text = raceTimeTitle;

    }

    public void ShowLapTimes()
    {
        ScoreServerManager.OnTrackDataUpdate -= UpdateHighScores;
        ScoreServerManager.OnLapDataUpdate += UpdateHighScores;
        currentlyShowing = HighScoreType.LapTimes;
        // Update high scores from server's data.
        UpdateHighScores(ScoreServerManager.Instance.GetLapData());
        title.text = lapTimeTitle;
    }

    private void OnDisable()
    {
        ScoreServerManager.OnTrackDataUpdate -= UpdateHighScores;
        ScoreServerManager.OnLapDataUpdate -= UpdateHighScores;
    }

    public void UpdateHighScores(HighScoreCollection collection)
    {
        // Go through each entry
        for (int i = 0; i < entries.Count; i++)
        {
            // if the high score exists for this entry
            if (i < collection.highScores.Length)
            {
                entries[i].SetName(collection.highScores[i].name);
                entries[i].SetTime(collection.highScores[i].timeSeconds);
            }
            
        }
    }
    
    
}
