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

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        entries.Clear();
        for (int i = 0; i < highScoreContainer.childCount; i++)
        {
            entries.Add(highScoreContainer.GetChild(i).GetComponent<HighScoreEntry>());
        }

        UpdateNumbers();

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
    private void UpdateNumbers()
    {
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
        UpdateNumbers();
        // Go through each entry
        for (int i = 0; i < entries.Count; i++)
        {
            // if the high score exists for this entry
            if (collection.highScores != null && i < collection.highScores.Length)
            {
                entries[i].SetName(collection.highScores[i].user);
                entries[i].SetTime(collection.highScores[i].score);
            }
        }
    }
    
    
}
