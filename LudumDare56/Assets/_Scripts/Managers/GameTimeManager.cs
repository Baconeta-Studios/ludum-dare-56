using System;
using _Scripts.Racer;
using UnityEngine;
using Utils;

namespace _Scripts.Managers
{
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        private bool isRacing;
        
        // Total time since the race began, updated each Update().
        private float totalRaceTimeSoFar;
        // Current Lap time.
        private float totalLapTimeSoFar;
        // Fastest Lap.
        private float fastestLap;
        
        private float lastTimeFinishLineCrossed; // Used to calculate lap times.
        
        public static event Action<float> OnRaceTimeChanged;
        public static event Action<float> OnLapTimeChanged;
        public static event Action<float> OnFastestLapTimeChanged;
        public static event Action<float, float> OnRaceFinished;
        
        public void StartRaceTimer()
        {
            // Reset timer.
            totalRaceTimeSoFar = 0;
            totalLapTimeSoFar = 0;
            fastestLap = 0;
            lastTimeFinishLineCrossed = 0;
            
            // Start timer.
            isRacing = true;
        }

        public void StopRaceTimer()
        {
            isRacing = false;
            OnRaceFinished?.Invoke(totalRaceTimeSoFar, totalLapTimeSoFar);
        }

        public void HandleLapEndEvent(RacerBase racer)
        {
            if (racer.GetType() != typeof(RacerPlayer))
            {
                return;
            }
            
            if (!isRacing)
            {
                StartRaceTimer();
                return;
            }
            lastTimeFinishLineCrossed = totalRaceTimeSoFar;
            
            // Update fastest-lap time.
            if (totalLapTimeSoFar > fastestLap)
            {
                fastestLap = totalLapTimeSoFar;
                OnFastestLapTimeChanged?.Invoke(fastestLap);
            }

            // Reset lap timer.
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
            if (!isRacing) return;
            
            totalRaceTimeSoFar += Time.deltaTime;
            totalLapTimeSoFar = totalRaceTimeSoFar - lastTimeFinishLineCrossed;
            OnRaceTimeChanged?.Invoke(totalRaceTimeSoFar);
            OnLapTimeChanged?.Invoke(totalLapTimeSoFar);
        }
    }
}