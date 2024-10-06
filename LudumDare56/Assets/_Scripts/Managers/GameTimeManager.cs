using System;
using _Scripts.Racer;
using UnityEngine;

namespace _Scripts.Managers
{
    public class GameTimeManager : MonoBehaviour
    {
        private bool isRacing;
        
        private float totalRaceTimeSoFar; // Total time since the race began, updated each Update().
        private float lastTimeFinishLineCrossed; // Used to calculate lap times.
        // Last Lap.
        private float lastLapTime;
        // Fastest Lap.
        private float fastestLap;
        
        
        public void StartRaceTimer()
        {
            // Reset timer.
            totalRaceTimeSoFar = 0;
            lastTimeFinishLineCrossed = 0;
            lastLapTime = 0;
            fastestLap = 0;
            
            // Start timer.
            isRacing = true;
        }

        public void StopRaceTimer()
        {
            isRacing = false;
        }
        
        public TimeSpan GetRaceTime()
        {
            return TimeSpan.FromSeconds(fastestLap);
        }

        public void HandleLapEndEvent(RacerBase racer)
        {
            if (!isRacing)
            {
                StartRaceTimer();
                return;
            }
            // Calculate last-lap time.
            lastLapTime = totalRaceTimeSoFar - lastTimeFinishLineCrossed;
            lastTimeFinishLineCrossed = totalRaceTimeSoFar;
            
            // Update fastest-lap time.
            if (lastLapTime < fastestLap)
            {
                fastestLap = lastLapTime;
            }
            
            // Update cached times.
            lastLapTimeCache = TimeSpan.FromSeconds(lastLapTime);
            fastestLapCache = TimeSpan.FromSeconds(fastestLap);
        }

        private void OnEnable()
        {
            FinishLine.OnRacerCrossFinishLine += HandleLapEndEvent;
        }

        private void OnDisable()
        {
            FinishLine.OnRacerCrossFinishLine -= HandleLapEndEvent;
        }
        
        private void Update()
        {
            if (isRacing)
            {
                totalRaceTimeSoFar += Time.deltaTime;
            }
        }
    }
}