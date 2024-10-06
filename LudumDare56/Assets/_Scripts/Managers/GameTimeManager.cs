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
        private TimeSpan lastLapTimeCache;
        // Fastest Lap.
        private float fastestLap;
        private TimeSpan fastestLapCache;
        
        
        public void StartRaceTimer()
        {
            // Reset timer.
            totalRaceTimeSoFar = 0;
            lastTimeFinishLineCrossed = 0;
            lastLapTime = 0;
            lastLapTimeCache = TimeSpan.Zero;
            fastestLap = 0;
            fastestLapCache = TimeSpan.Zero;
            
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

        public TimeSpan GetLastLapTime()
        {
            return lastLapTimeCache;
        }

        public TimeSpan GetFastestLapTime()
        {
            return fastestLapCache;
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