using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Serialization;

public class HighScoreTab : MonoBehaviour
{
    [Serializable]
    public enum HighScoreType
    {
        RaceTimes,
        LapTimes,
    }

    public HighScoreType currentlyShowing;
    
    [Serializable]
    public struct HighScoreNewEntry
    {
        public string name;
        public int timeSeconds;

    }
    public TextMeshProUGUI title;
    public string lapTimeTitle;
    public string raceTimeTitle;
    public Transform highScoreContainer;
    public List<HighScoreEntry> entries;

    [ContextMenu("Refresh Numbers")]
    public void UpdateNumbers()
    {
        entries.Clear();
        for (int i = 0; i < highScoreContainer.childCount; i++)
        {
            entries.Add(highScoreContainer.GetChild(i).GetComponent<global::HighScoreEntry>());
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
        // @james
        // highscoreEntires = new List<HighScoreNewEntry>()
        // foreach entry > entries.add(new HighScoreNewEntry(name, timeSecondsInt))
        // UpdateHighScores(highscoreEntires)
        title.text = raceTimeTitle;

    }

    public void ShowLapTimes()
    {
        currentlyShowing = HighScoreType.LapTimes;
        // @james
        // highscoreEntires = new List<HighScoreNewEntry>()
        // foreach entry > entries.add(new HighScoreNewEntry(name, timeSecondsInt))
        // UpdateHighScores(highscoreEntries)
        title.text = lapTimeTitle;
    }

    public void UpdateHighScores(HighScoreNewEntry[] highScores)
    {
        // Go through each entry
        for (int i = 0; i < entries.Count; i++)
        {
            // if the high score exists for this entry
            if (i < highScores.Length)
            {
                entries[i].SetName(highScores[i].name);
                entries[i].SetTime(highScores[i].timeSeconds);
            }
            
        }
    }
    
    
}
