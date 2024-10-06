using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Managers
{
    public class TrackPositionManager : MonoBehaviour
    {
        private RacerProgress<RacerPlayer> playerRacer;
        private RacerProgress<AiRacer>[] otherRacers;
        
        private void Start()
        {
            List<RacerBase> racers = FindObjectsByType<RacerBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            
            // Find and extract the player racer from the list of racers.
            foreach (var racerPlayer in racers.Where(racer => racer.GetType() == RacerPlayer.GetType()))
            {
                this.playerRacer = new RacerProgress<RacerPlayer>((RacerPlayer) racerPlayer);
                racers.Remove(racerPlayer);
                break;
            }
            
            // Save other racers, they'll all be AI.
            for (int i = 0; i < racers.Count; i++)
            {
                otherRacers = new RacerProgress<AiRacer>[racers.Count];
                otherRacers[i] = new RacerProgress<AiRacer>((AiRacer) racers[i]);
            }
        }
    }

    public class RacerProgress<T> : IComparable<RacerProgress<T>>
    {
        private T racer;
        private int lapsCompleted;
        private float lapProgress;

        public RacerProgress(T racer)
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
        
        public int CompareTo(RacerProgress<T> other)
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