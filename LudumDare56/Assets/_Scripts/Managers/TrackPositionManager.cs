using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts.Managers
{
    public class TrackPositionManager : MonoBehaviour
    {
        private RacerProgress player;
        private List<RacerProgress> otherRacers;
        
        public static event Action<int> OnPlayerPositionChanged;
        public static event Action<int> OnPlayerLapCompleted;
        
        private void Start()
        {
            List<RacerBase> racers = FindObjectsByType<RacerBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            
            // Find and extract the player racer from the list of racers.
            foreach (var racerPlayer in racers.Where(racer => racer.GetType() == typeof(RacerPlayer)))
            {
                this.player = new RacerProgress((RacerPlayer) racerPlayer);
                racers.Remove(racerPlayer);
                break;
            }
            
            // Save other racers, they'll all be AI.
            for (int i = 0; i < racers.Count; i++)
            {
                otherRacers = new List<RacerProgress>(racers.Count)
                {
                    [i] = new RacerProgress((RacerAi) racers[i])
                };
            }
        }
        
        private void OnEnable()
        {
            FinishLine.OnRacerCrossFinishLine += HandleLapEndEvent;
        }

        private void OnDisable()
        {
            FinishLine.OnRacerCrossFinishLine -= HandleLapEndEvent;
        }
        
        private void HandleLapEndEvent(RacerBase racer)
        {
            if (racer.GetType() == typeof(RacerPlayer))
            {
                player.IncrementLapsCompleted();
                OnPlayerLapCompleted?.Invoke((int) Math.Round(player.GetRaceProgress()));
            }
            else
            {
                foreach (var racerProgress in otherRacers.Where(racerProgress => racerProgress.Racer.Equals(racer)))
                {
                    racerProgress.IncrementLapsCompleted();
                }
            }
        }
        
        /// <summary>
        /// Returns the place that the player is in, relative to other races. 1=1st, 3=3rd, etc.
        /// </summary>
        /// <returns>Player's relative position in the race</returns>
        public int GetPlayerPosition() {
            List<RacerProgress> li = otherRacers.ToList();
            li.Add(player);
            li.Sort();
            return li.IndexOf(player) + 1;
        }

    }
    
    public class RacerProgress : IComparable<RacerProgress>
    {
        public readonly RacerBase Racer;
        private int lapsCompleted;
        private float lapProgress;

        public RacerProgress(RacerBase racer)
        {
            this.Racer = racer;
            lapsCompleted = 0;
            lapProgress = 0;
        }
        
        public void IncrementLapsCompleted()
        {
            lapsCompleted++;
        }

        public void UpdateLapProgress(float progress)
        {
            lapProgress = progress;
        }
        
        public float GetRaceProgress()
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