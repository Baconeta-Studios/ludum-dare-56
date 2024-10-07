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
        ShowTrackTimes();
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
        currentlyShowing = HighScoreType.RaceTimes;
        // Update high scores from server's data.
        UpdateHighScores(ScoreServerManager.Instance.trackData);
        title.text = raceTimeTitle;

    }

    public void ShowLapTimes()
    {
        currentlyShowing = HighScoreType.LapTimes;
        // Update high scores from server's data.
        UpdateHighScores(ScoreServerManager.Instance.lapData);
        title.text = lapTimeTitle;
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
