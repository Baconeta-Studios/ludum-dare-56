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
        
        public static event Action<int> OnPlayerRankingChanged;
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
            otherRacers = racers.ToList().ConvertAll(racer => new RacerProgress(racer));
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
                OnPlayerLapCompleted?.Invoke(Mathf.Max(player.GetCurrentLap(), 1));
            }
            else
            {
                foreach (var aiRacer in otherRacers.Where(aiRacer => aiRacer.Racer.Equals(racer)))
                {
                    aiRacer.IncrementLapsCompleted();
                    break;
                }
            }
        }

        private void Update()
        {
            UpdatePlayerRanking();
        }
        
        /// <summary>
        /// Calculates the ranking that the player is in, relative to other racers. e.g. 1st, 3rd.
        /// Broadcasts the ranking via OnPlayerRankingChanged.
        /// </summary>
        private void UpdatePlayerRanking() {
            // Ensure that our data is fresh.
            player.UpdateLapProgress();
            otherRacers.ForEach(racer => racer.UpdateLapProgress());
            
            // Make the ranking calculation.
            var li = new List<RacerProgress>(otherRacers) { player };
            li.Sort();
            OnPlayerRankingChanged?.Invoke(li.IndexOf(player) + 1);
        }
    }
    
    public class RacerProgress : IComparable<RacerProgress>
    {
        public readonly RacerBase Racer;
        private int currentLap;
        private float lapProgress;

        public RacerProgress(RacerBase racer)
        {
            this.Racer = racer;
            currentLap = 0;
            lapProgress = 0;
        }
        
        public void IncrementLapsCompleted()
        {
            currentLap++;
        }

        public void UpdateLapProgress()
        {
            lapProgress = Racer.DistanceAlongTrack;
        }

        public int GetCurrentLap()
        {
            return currentLap;
        }
        
        public float GetRaceProgress()
        {
            return GetCurrentLap() + lapProgress;
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