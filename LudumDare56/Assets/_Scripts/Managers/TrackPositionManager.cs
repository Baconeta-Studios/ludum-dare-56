using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts.Managers
{
    public class TrackPositionManager : MonoBehaviour
    {
        private RacerProgress playerRacer;
        private RacerProgress[] otherRacers;
        
        private void Start()
        {
            List<RacerBase> racers = FindObjectsByType<RacerBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            
            // Find and extract the player racer from the list of racers.
            foreach (var racerPlayer in racers.Where(racer => racer.GetType() == typeof(RacerPlayer)))
            {
                this.playerRacer = new RacerProgress((RacerPlayer) racerPlayer);
                racers.Remove(racerPlayer);
                break;
            }
            
            // Save other racers, they'll all be AI.
            for (int i = 0; i < racers.Count; i++)
            {
                otherRacers = new RacerProgress[racers.Count];
                otherRacers[i] = new RacerProgress((RacerAi) racers[i]);
            }
        }
        
        public int GetPlayerPosition() {
            List<RacerProgress> li = otherRacers.ToList();
            li.Add(playerRacer);
            li.Sort();
            return li.IndexOf(playerRacer) + 1;
        }
    }
    
    public class RacerProgress : IComparable<RacerProgress>
    {
        private RacerBase racer;
        private int lapsCompleted;
        private float lapProgress;

        public RacerProgress(RacerBase racer)
        {
            this.racer = racer;
            lapsCompleted = 0;
            lapProgress = 0;
        }

        public void IncrementLapsCompleted()
        {
            lapsCompleted++;
        }

        public void UpdateLapProgress(float progress)
        {
            this.lapProgress = progress;
        }
        
        private float GetRaceProgress()
        {
            return (lapsCompleted * 1.0F) + lapProgress;
        }
        
        public int CompareTo(RacerProgress other)
        {
            if (Mathf.Approximately(this.GetRaceProgress(), other.GetRaceProgress()))
            {
                return 0;
            }
            else
            {
                return this.GetRaceProgress() < other.GetRaceProgress() ? 1 : -1;
            }
        }
    }
}