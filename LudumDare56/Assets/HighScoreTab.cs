using System;
using System.Collections;
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


    public float reCheckTime = 1.1f;
    private bool trackTimesUpdated = false;
    private bool lapTimesUpdated = false;

    void OnEnable()
    {
        UpdateCurrentDisplay();
    }

    private void UpdateCurrentDisplay()
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
                ShowTrackTimes(true);
                break;
            case HighScoreType.LapTimes:
                ShowLapTimes(true);
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

    public void ShowTrackTimes(bool dontFetch=false)
    {
        if (!trackTimesUpdated && !dontFetch)
        {
            trackTimesUpdated = true;
            ScoreServerManager.OnLapDataUpdate -= UpdateHighScores;
            ScoreServerManager.OnTrackDataUpdate += UpdateHighScores;
            ScoreServerManager.Instance.GetTrackData();
        }
        else
        {
            UpdateHighScores(ScoreServerManager.Instance.GetTrackData(false));
        }
        currentlyShowing = HighScoreType.RaceTimes;
        title.text = raceTimeTitle;

    }

    public void ShowLapTimes(bool dontFetch=false)
    {
        if (!lapTimesUpdated && !dontFetch)
        {
            lapTimesUpdated = true;
            ScoreServerManager.OnLapDataUpdate += UpdateHighScores;
            ScoreServerManager.OnTrackDataUpdate -= UpdateHighScores;
            // Update high scores from server's data.
            ScoreServerManager.Instance.GetLapData();
        }
        else
        {
            UpdateHighScores(ScoreServerManager.Instance.GetLapData(false));
        }
        currentlyShowing = HighScoreType.LapTimes;
        title.text = lapTimeTitle;
    }

    private void OnDisable()
    {
        ScoreServerManager.OnTrackDataUpdate -= UpdateHighScores;
        ScoreServerManager.OnLapDataUpdate -= UpdateHighScores;
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
            else
            {
                entries[i].SetName("_________");
                entries[i].SetTime(0);
            }
        }
    }
    
    
}
